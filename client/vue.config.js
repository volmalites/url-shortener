const { defineConfig } = require('@vue/cli-service');
const TerserPlugin = require('terser-webpack-plugin');
const MiniCssExtractPlugin = require('mini-css-extract-plugin');

module.exports = defineConfig({
	transpileDependencies: true,
	publicPath: '/',
	outputDir: 'dist',
	devServer: {
		proxy: {
			'/api': {
				target: 'http://localhost:1337',
				changeOrigin: true
			}
		}
	},
	configureWebpack: {
		optimization: {
			minimizer: [
				new TerserPlugin({
					terserOptions: {
						compress: {
							drop_console: true,
						},
					},
				}),
			],
		},
		plugins: [
			new MiniCssExtractPlugin({
				filename: '[name].[contenthash].css',
			}),
		],
	},
})
