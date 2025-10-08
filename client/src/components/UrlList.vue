<template>
	<div class="url-list">
		<h3>Your Links</h3>
		<ul>
			<li v-for="url in urls" :key="url.shortCode">
				ShortCode: <a :href="'http://localhost:1337/' + url.shortCode" target="_blank">{{ url.shortCode }}</a>,
				Raw Short Link: <a :href="'http://localhost:1337/' + url.shortCode" target="_blank">{{ 'http://localhost:1337/' + url.shortCode }}</a>,
				Original: <a :href="url.originalUrl" target="_blank">{{ url.originalUrl }}</a>,
				Clicks: {{ url.clicks }}
				<button @click="simulateClick(url.shortCode)" class="btn">Simulate Click</button>
			</li>
		</ul>
	</div>
</template>

<script>
import axios from 'axios';

export default {
	props: {
		urls: {
			type: Array,
			default: () => []
		}
	},
	methods: {
		async simulateClick(shortCode) {
			try {
				await axios.post(`/api/urls/click/${shortCode}`);
				this.$emit('url-clicked');
				alert(`Simulated click on ${shortCode}`);
			} catch (err) {
				console.error(`Failed to simulate click on ${shortCode}:`, err);
				alert('Error simulating click: ' + (err.response?.data || err.message));
			}
		}
	}
};
</script>

<style scoped>
.url-list {
	margin-bottom: 20px;
}
ul {
	list-style-type: none;
	padding: 0;
}
li {
	margin: 10px 0;
}
.btn {
	margin-left: 10px;
}
</style>
