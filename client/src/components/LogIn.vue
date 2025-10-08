<template>
	<div class="auth-container">
		<h2 v-if="!isAuthenticated">Login</h2>
		<h2 v-else>Logout</h2>
		<div v-if="!isAuthenticated">
			<input v-model="username" placeholder="Username" class="input-field" />
			<input v-model="password" type="password" placeholder="Password" class="input-field" />
			<button @click="login" class="btn">Login</button>
			<p><router-link to="/register">Need an account? Register</router-link></p>
		</div>
		<div v-else>
			<button @click="logout" class="btn logout-btn">Logout</button>
		</div>
	</div>
</template>

<script>
import axios from 'axios';

export default {
	data() {
		return {
			username: '',
			password: '',
			isAuthenticated: !!localStorage.getItem('auth')
		};
	},
	methods: {
		async login() {
			try {
				localStorage.setItem('auth', btoa(`${this.username}:${this.password}`));
				axios.defaults.headers.common['Authorization'] = 'Basic ' + localStorage.getItem('auth');
				this.isAuthenticated = true;
				this.$emit('logged-in');
				this.$router.push('/dashboard');
			} catch (err) {
				alert('Login failed: ' + err.message);
			}
		},
		logout() {
			localStorage.removeItem('auth');
			axios.defaults.headers.common['Authorization'] = '';
			this.isAuthenticated = false;
			this.username = '';
			this.password = '';
			this.$emit('logged-out');
			this.$router.push('/login');
		}
	}
};
</script>

<style scoped>
.auth-container {
	max-width: 800px;
	margin: 20px auto;
	padding: 20px;
	text-align: center;
}
.input-field {
	display: block;
	margin: 10px auto;
}
.logout-btn {
	margin: 20px auto;
}
</style>
