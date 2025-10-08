<template>
	<div>
		<h3>Wallet</h3>
		<p>Balance: R{{ wallet.Balance.toFixed(2) }}</p>
		<p>Current Percentage: {{ wallet.Percentage }}%</p>
		<p>{{ wallet.GrowthInfo }}</p>
	</div>
</template>

<script>
export default {
	props: {
		wallet: Object
	}
};
</script>
