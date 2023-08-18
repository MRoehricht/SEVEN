<script lang="ts">
	import 'carbon-components-svelte/css/all.css';
	import '@carbon/styles/css/styles.css';
	import '@carbon/charts-svelte/styles.css';
	import type { DqlProperty, Measurement, Probe, DqlToken } from '$lib/types';
	import DashboardToolbar from '$lib/components/DashboardToolbar.svelte';
	import DeviceQueryLanguageBuilder from '$lib/components/DeviceQueryLanguageBuilder.svelte';
	import { env } from '$env/dynamic/public';
	import { onMount } from 'svelte';

	let isLoading = false;
	let fetchError = '';
	let measurements: Measurement[] = [];
	let dqlProperties: DqlProperty[] = [];

	async function executeDql(tokens: DqlToken[]) {
		isLoading = true;

		const options: RequestInit = {
			method: 'POST',
			headers: { 'Content-Type': 'application/json', accept: '*/*' },
			body: JSON.stringify(tokens)
		};

		measurements = await fetch(`${env.PUBLIC_API_URL}/device/dql`, options)
			.then((res) => res.json())
			.catch((err) => {
				fetchError = err.message;
			});

		isLoading = false;
	}

	async function getDqlProperties() {
		isLoading = true;

		const options: RequestInit = {
			method: 'GET',
			headers: { 'Content-Type': 'application/json', accept: '*/*' }
		};

		dqlProperties = await fetch(`${env.PUBLIC_API_URL}/device/dql/properties`, options)
			.then((res) => res.json())
			.catch((err) => {
				fetchError = err.message;
			});

		isLoading = false;
	}

	onMount(() => {
		getDqlProperties();
	});
</script>

<DashboardToolbar
	title="Explore"
	crumbs={[
		{ label: 'Home', path: '/' },
		{ label: 'Devices', path: '/devices' },
		{ label: 'Explore', path: '/devices/explore' }
	]}
/>

<DeviceQueryLanguageBuilder bind:dqlProperties onExecuteQuery={executeDql} />

{#if isLoading}
	<div>Loading...</div>
{:else if fetchError}
	<div>{fetchError}</div>
{:else if measurements.length > 0}
	<div>
		{#each measurements as measurement}
			<div>{JSON.stringify(measurement, null, 2)}</div>
		{/each}
	</div>
{:else}
	<div>No results</div>
{/if}
