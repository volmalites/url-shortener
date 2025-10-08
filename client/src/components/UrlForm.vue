<template>
	<div>
		<h3>Create Shortened URL</h3>
		<input v-model="originalUrl" placeholder="Original URL (e.g., https://example.com)" />
		<button @click="createUrl">Create</button>
		<p v-if="errorMessage" style="color: red;">{{ errorMessage }}</p>
		<p v-if="newShortUrl">New Short URL: {{ newShortUrl }}</p>
	</div>
</template>

<script>
import axios from 'axios';

export default {
	data() {
		return {
			originalUrl: '',
			newShortUrl: '',
			errorMessage: ''
		};
	},
	methods: {
		validateUrl(string) {
			if (!string.trim()) {
				return { valid: false, error: 'Please enter a URL' };
			}

			let url;
			try {
				url = new URL(string);
			} catch (_) {
				return { valid: false, error: 'Invalid URL format. Please include http:// or https://' };
			}

			if (url.protocol !== 'http:' && url.protocol !== 'https:') {
				return { valid: false, error: 'URL must start with http:// or https://' };
			}

			const host = url.hostname;

			if (host === 'localhost') {
				return { valid: true };
			}

			if (!host.includes('.')) {
				return { valid: false, error: 'URL must have a valid domain (e.g., example.com)' };
			}

			const parts = host.split('.');
			if (parts.length < 2 || parts[parts.length - 1].length < 2) {
				return { valid: false, error: 'URL must have a valid domain extension (e.g., .com, .org)' };
			}

			const validHostnameRegex = /^[a-zA-Z0-9.-]+$/;
			if (!validHostnameRegex.test(host)) {
				return { valid: false, error: 'Domain contains invalid characters' };
			}

			return { valid: true };
		},
		async createUrl() {
			this.errorMessage = '';
			this.newShortUrl = '';

			const validation = this.validateUrl(this.originalUrl);
			if (!validation.valid) {
				this.errorMessage = validation.error;
				return;
			}

			try {
				const res = await axios.post('http://localhost:1337/api/urls', { OriginalUrl: this.originalUrl });
				this.newShortUrl = res.data.shortUrl;
				this.originalUrl = '';
				this.$emit('url-created');
			} catch (err) {
				if (err.response && err.response.data && err.response.data.error) {
					this.errorMessage = err.response.data.error;
				} else {
					this.errorMessage = 'Error creating URL: ' + err.message;
				}
			}
		}
	}
};
</script>
