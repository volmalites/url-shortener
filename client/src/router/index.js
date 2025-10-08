import { createRouter, createWebHistory } from 'vue-router';
import LogIn from '../components/LogIn.vue';
import RegisterPage from '../components/RegisterPage.vue';
import DashBoard from '../components/DashBoard.vue';

const routes = [
	{ path: '/', redirect: '/login' },
	{ path: '/login', component: LogIn },
	{ path: '/register', component: RegisterPage },
	{ path: '/dashboard', component: DashBoard }
];

const router = createRouter({
	history: createWebHistory(),
	routes
});

router.beforeEach((to, from, next) => {
	const isAuthenticated = !!localStorage.getItem('auth');
	if (to.path !== '/login' && to.path !== '/register' && !isAuthenticated) {
		next('/login');
	} else {
		next();
	}
});

export default router;
