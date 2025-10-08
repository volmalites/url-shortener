<template>
	<div id="app">
		<router-view @logged-in="checkAuth" @logged-out="checkAuth"></router-view>
	</div>
</template>

<script>
import axios from 'axios';

export default {
	methods: {
		checkAuth() {
			const auth = localStorage.getItem('auth');
			if (auth) {
				axios.defaults.headers.common['Authorization'] = 'Basic ' + auth;
			} else {
				axios.defaults.headers.common['Authorization'] = '';
			}
		}
	},
	mounted() {
		this.checkAuth();
	}
};
</script>
