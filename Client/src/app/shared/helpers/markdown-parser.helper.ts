import { marked } from 'marked';
import * as katex from 'katex';

export function parseMarkdownToHtml(markdownText: string): string {
  if (!markdownText) return '';

  const mathPlaceholders: string[] = [];

  // 1. Extract block math $$...$$ to prevent marked from mangling it
  let processed = markdownText.replace(/\$\$([\s\S]+?)\$\$/g, (match, formula) => {
    try {
      const rendered = katex.renderToString(formula.trim(), { displayMode: true, throwOnError: false });
      const placeholder = `<!--MATH_BLOCK_${mathPlaceholders.length}-->`;
      mathPlaceholders.push(rendered);
      return placeholder;
    } catch (e) {
      return match;
    }
  });

  // 2. Extract inline math $...$ to prevent marked from mangling it
  processed = processed.replace(/\$([^\$\n]+?)\$/g, (match, formula) => {
    try {
      const rendered = katex.renderToString(formula.trim(), { displayMode: false, throwOnError: false });
      const placeholder = `<!--MATH_INLINE_${mathPlaceholders.length}-->`;
      mathPlaceholders.push(rendered);
      return placeholder;
    } catch (e) {
      return match;
    }
  });

  // 3. Preprocess Admonitions, Tabs, and Custom Components
  processed = preprocessAdmonitions(processed);
  processed = preprocessTabs(processed);
  processed = preprocessCustomComponents(processed);

  // 4. Parse markdown with marked
  let html = marked.parse(processed, { async: false }) as string;

  // 5. Postprocess code blocks for Mermaid diagrams
  html = postprocessMermaid(html);

  // 6. Restore Math placeholders
  for (let i = 0; i < mathPlaceholders.length; i++) {
    html = html.replace(`<!--MATH_BLOCK_${i}-->`, mathPlaceholders[i]);
    html = html.replace(`<!--MATH_INLINE_${i}-->`, mathPlaceholders[i]);
  }

  return html;
}

function preprocessAdmonitions(text: string): string {
  const admonitionRegex = /:::(note|warning|tip|caution|important|info)\s*\n([\s\S]*?)\n:::/gi;
  return text.replace(admonitionRegex, (match, type, content) => {
    const cleanType = type.toLowerCase();
    const title = type.toUpperCase();
    
    let iconSvg = '';
    if (cleanType === 'note' || cleanType === 'info') {
      iconSvg = '<svg viewBox="0 0 24 24" width="16" height="16" stroke="currentColor" stroke-width="2.5" fill="none" style="margin-right: 6px;"><circle cx="12" cy="12" r="10"></circle><line x1="12" y1="16" x2="12" y2="12"></line><line x1="12" y1="8" x2="12.01" y2="8"></line></svg>';
    } else if (cleanType === 'warning' || cleanType === 'caution') {
      iconSvg = '<svg viewBox="0 0 24 24" width="16" height="16" stroke="currentColor" stroke-width="2.5" fill="none" style="margin-right: 6px;"><path d="M10.29 3.86L1.82 18a2 2 0 0 0 1.71 3h16.94a2 2 0 0 0 1.71-3L13.71 3.86a2 2 0 0 0-3.42 0z"></path><line x1="12" y1="9" x2="12" y2="13"></line><line x1="12" y1="17" x2="12.01" y2="17"></line></svg>';
    } else if (cleanType === 'tip') {
      iconSvg = '<svg viewBox="0 0 24 24" width="16" height="16" stroke="currentColor" stroke-width="2.5" fill="none" style="margin-right: 6px;"><path d="M9.663 17h4.673M12 3v1m0 16v1m8-9h-1M4 12H3m14.364-6.364l-.707.707M6.343 17.657l-.707.707m0-12.728l.707.707m10.607 10.607l.707-.707M12 5a7 7 0 0 0-7 7c0 1.83.707 3.5 1.864 4.757h10.272A7 7 0 0 0 12 5z"></path></svg>';
    } else if (cleanType === 'important') {
      iconSvg = '<svg viewBox="0 0 24 24" width="16" height="16" stroke="currentColor" stroke-width="2.5" fill="none" style="margin-right: 6px;"><circle cx="12" cy="12" r="10"></circle><line x1="12" y1="8" x2="12" y2="12"></line><line x1="12" y1="16" x2="12.01" y2="16"></line></svg>';
    }

    return `<div class="admonition admonition-${cleanType}">
  <div class="admonition-title">
    ${iconSvg}
    <span>${title}</span>
  </div>
  <div class="admonition-content">
    ${content}
  </div>
</div>`;
  });
}

function preprocessTabs(text: string): string {
  const tabsRegex = /:::tabs\s*\n([\s\S]*?)\n:::/gi;
  return text.replace(tabsRegex, (match, content) => {
    const parts = content.split(/@tab\s+/);
    const tabData: { title: string; body: string }[] = [];
    
    for (const part of parts) {
      if (!part.trim()) continue;
      const lines = part.split('\n');
      const title = lines[0].trim();
      const body = lines.slice(1).join('\n').trim();
      tabData.push({ title, body });
    }

    if (tabData.length === 0) return '';

    let buttonsHtml = '';
    let panelsHtml = '';

    tabData.forEach((tab, index) => {
      const activeClass = index === 0 ? 'active' : '';
      buttonsHtml += `<button class="tab-button ${activeClass}">${tab.title}</button>`;
      panelsHtml += `<div class="tab-panel ${activeClass}">${tab.body}</div>`;
    });

    return `<div class="custom-tabs-container">
  <div class="tabs-header">
    ${buttonsHtml}
  </div>
  <div class="tabs-content">
    ${panelsHtml}
  </div>
</div>`;
  });
}

function preprocessCustomComponents(text: string): string {
  // 1. Timeline
  const timelineRegex = /:::timeline\s*\n([\s\S]*?)\n:::/gi;
  let processed = text.replace(timelineRegex, (match, content) => {
    const items = content.split('\n').map((line: string) => line.trim()).filter((line: string) => line.startsWith('-'));
    let timelineHtml = '<div class="custom-timeline-container">';
    for (const item of items) {
      const cleanItem = item.substring(1).trim();
      const parts = cleanItem.split(/\s+-\s+|\s*:\s*/);
      const datePart = parts[0] || '';
      const descPart = parts.slice(1).join(' - ') || '';
      timelineHtml += `
        <div class="timeline-item">
          <div class="timeline-dot"></div>
          <div class="timeline-content">
            <span class="timeline-date">${datePart}</span>
            <p class="timeline-desc">${descPart}</p>
          </div>
        </div>`;
    }
    timelineHtml += '</div>';
    return timelineHtml;
  });

  // 2. Steps
  const stepsRegex = /:::steps\s*\n([\s\S]*?)\n:::/gi;
  processed = processed.replace(stepsRegex, (match, content) => {
    const items = content.split('\n').map((line: string) => line.trim()).filter((line: string) => /^\d+\.\s*/.test(line));
    let stepsHtml = '<div class="custom-steps-container">';
    for (const item of items) {
      const matchNumber = item.match(/^(\d+)\.\s*(.*)/);
      if (matchNumber) {
        const num = matchNumber[1];
        const cleanItem = matchNumber[2].trim();
        const parts = cleanItem.split(/\s+-\s+|\s*:\s*/);
        const titlePart = parts[0] || '';
        const descPart = parts.slice(1).join(' - ') || '';
        
        stepsHtml += `
          <div class="step-item">
            <div class="step-number">${num}</div>
            <div class="step-content">
              <h5 class="step-title">${titlePart}</h5>
              <p class="step-desc">${descPart}</p>
            </div>
          </div>`;
      }
    }
    stepsHtml += '</div>';
    return stepsHtml;
  });

  // 3. Architecture Diagram Wrapper
  const archRegex = /:::architecture\s*\n([\s\S]*?)\n:::/gi;
  processed = processed.replace(archRegex, (match, content) => {
    return `<div class="custom-architecture-container">
      <div class="architecture-header">
        <svg viewBox="0 0 24 24" width="16" height="16" stroke="currentColor" stroke-width="2.5" fill="none" style="margin-right: 6px;"><rect x="3" y="3" width="18" height="18" rx="2" ry="2"></rect><line x1="9" y1="3" x2="9" y2="21"></line></svg>
        <span>SYSTEM ARCHITECTURE DIAGRAM</span>
      </div>
      <div class="architecture-content">
        ${content}
      </div>
    </div>`;
  });

  return processed;
}

function postprocessMermaid(html: string): string {
  const mermaidRegex = /<pre><code class="language-mermaid">([\s\S]*?)<\/code><\/pre>/gi;
  return html.replace(mermaidRegex, (match, code) => {
    const decoded = code
      .replace(/&lt;/g, '<')
      .replace(/&gt;/g, '>')
      .replace(/&amp;/g, '&')
      .replace(/&quot;/g, '"')
      .replace(/&#39;/g, "'");
    return `<div class="mermaid">${decoded}</div>`;
  });
}
