<template>
	<div class="wallet">
		<h3>Wallet</h3>
		<p v-if="wallet">Balance: R{{ wallet.balance?.toFixed(2) || '0.00' }}</p>
		<p v-if="wallet">Current Percentage: {{ wallet.percentage }}%</p>
		<p v-if="wallet">{{ wallet.growthInfo }}</p>
		<p v-else>Loading wallet...</p>
	</div>
</template>

<script>
export default {
	props: {
		wallet: {
			type: Object,
			default: () => ({ Balance: 0, Percentage: 10, GrowthInfo: '' })
		}
	}
};
</script>

<style scoped>
.wallet {
	margin-bottom: 20px;
}
</style>
