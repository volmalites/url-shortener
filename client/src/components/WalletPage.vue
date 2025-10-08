<template>
	<div class="wallet">
		<h3>Wallet</h3>
		<p v-if="wallet">Total earnings: R {{ wallet.totalBalance?.toFixed(2) || '0.00' }}</p>
		<p v-if="wallet">Your balance: R {{ wallet.balance?.toFixed(2) || '0.00' }}</p>
		<p v-if="wallet">{{ wallet.growthInfo }}</p>
		<p v-if="wallet">Current total clicks: {{ wallet.totalClicks || 0 }}</p>
		<p v-else>Loading wallet...</p>
	</div>
</template>

<script>
export default {
	props: {
		wallet: {
			type: Object,
			default: () => ({ Balance: 0, TotalBalance: 0, Percentage: 10, GrowthInfo: '', TotalClicks: 0 })
		}
	}
};
</script>

<style scoped>
.wallet {
	margin-bottom: 20px;
}
</style>
