<template>
	<div class="dashboard">
		<h2>Dashboard</h2>
		<button @click="logout" class="btn logout-btn">Logout</button>
		<UrlForm @url-created="loadData" />
		<UrlList v-if="!loading" :urls="urls" @url-clicked="loadData" />
		<WalletPage v-if="!loading" :wallet="wallet" />
		<p v-if="loading">Loading...</p>
		<button @click="loadData" class="btn">Refresh</button>
	</div>
</template>

<script>
import axios from 'axios';
import UrlForm from './UrlForm.vue';
import UrlList from './UrlList.vue';
import WalletPage from './WalletPage.vue';

export default {
	components: { UrlForm, UrlList, WalletPage },
	data() {
		return {
			urls: [],
			wallet: null,
			loading: false
		};
	},
	mounted() {
		this.loadData();
	},
	methods: {
		async loadData() {
			this.loading = true;
			try {
				const urlsRes = await axios.get('/api/urls');
				this.urls = urlsRes.data;
				const walletRes = await axios.get('/api/wallet');
				this.wallet = walletRes.data;
			} catch (err) {
				alert('Error loading data: ' + (err.response?.data || err.message));
				this.logout();
			} finally {
				this.loading = false;
			}
		},
		logout() {
			localStorage.removeItem('auth');
			axios.defaults.headers.common['Authorization'] = '';
			this.$router.push('/login');
		}
	}
};
</script>

<style scoped>
.dashboard {
	max-width: 800px;
	margin: 20px auto;
	padding: 20px;
}
.logout-btn {
	margin-bottom: 20px;
}
</style>
