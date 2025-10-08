<template>
	<div class="auth-container">
		<h2>Register</h2>
		<div>
			<input v-model="username" placeholder="Username" class="input-field" />
			<input v-model="password" type="password" placeholder="Password" class="input-field" />
			<button @click="register" class="btn">Register</button>
			<p><router-link to="/login">Already have an account? Login</router-link></p>
		</div>
	</div>
</template>

<script>
import axios from 'axios';

export default {
	data() {
		return {
			username: '',
			password: ''
		};
	},
	methods: {
		async register() {
			try {
				await axios.post('/api/auth/register', {
					Username: this.username,
					Password: this.password
				});
				alert('Registered successfully');
				this.$router.push('/login');
			} catch (err) {
				alert('Registration failed: ' + (err.response?.data || err.message));
			}
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
</style>
