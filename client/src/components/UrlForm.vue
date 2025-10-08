<template>
	<div>
		<h3>Create Shortened URL</h3>
		<input v-model="originalUrl" placeholder="Original URL" />
		<button @click="createUrl">Create</button>
		<p v-if="newShortUrl">New Short URL: {{ newShortUrl }}</p>
	</div>
</template>

<script>
import axios from 'axios';

export default {
	data() {
		return {
			originalUrl: '',
			newShortUrl: ''
		};
	},
	methods: {
		async createUrl() {
			try {
				const res = await axios.post('http://localhost:1337/api/urls', { OriginalUrl: this.originalUrl });
				this.newShortUrl = res.data.ShortUrl;
				this.$emit('url-created');
			} catch (err) {
				alert('Error creating URL: ' + err.message);
			}
		}
	}
};
</script>
