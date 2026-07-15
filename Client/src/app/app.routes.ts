import { Routes } from '@angular/router';
import { HomeLayoutComponent } from './shared/layouts/home-layout/home-layout.component';
import { AdminLayoutComponent } from './shared/layouts/admin-layout/admin-layout.component';
import { HomeComponent } from './pages/home/landing/home.component';
import { AboutComponent } from './pages/home/about/about.component';
import { PortfolioComponent } from './pages/home/portfolio/portfolio.component';
import { ProjectDetailComponent } from './pages/home/project-detail/project-detail.component';
import { BlogComponent } from './pages/home/blog/blog.component';
import { BlogDetailComponent } from './pages/home/blog-detail/blog-detail.component';
import { AdminComponent } from './pages/admin/admin.component';
import { ResetPasswordComponent } from './pages/reset-password/reset-password.component';

export const routes: Routes = [
  {
    path: '',
    component: HomeLayoutComponent,
    children: [
      { path: '', component: HomeComponent },
      { path: 'about', component: AboutComponent },
      { path: 'portfolio', component: PortfolioComponent },
      { path: 'portfolio/:id', component: ProjectDetailComponent },
      { path: 'blog', component: BlogComponent },
      { path: 'blog/:id', component: BlogDetailComponent }
    ]
  },
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      { path: 'admin', component: AdminComponent },
      { path: 'reset-password', component: ResetPasswordComponent }
    ]
  },
  {
    path: '**',
    redirectTo: ''
  }
];
