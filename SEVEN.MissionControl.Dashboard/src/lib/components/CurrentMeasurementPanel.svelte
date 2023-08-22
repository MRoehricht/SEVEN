<script lang="ts">
	import { Tile } from 'carbon-components-svelte';
	import TemperatureCelsius from 'carbon-icons-svelte/lib/TemperatureCelsius.svelte';
	import HumidityAlt from 'carbon-icons-svelte/lib/HumidityAlt.svelte';
	import SoilMoisture from 'carbon-icons-svelte/lib/SoilMoisture.svelte';
	import CloudDownload from 'carbon-icons-svelte/lib/CloudDownload.svelte';
	import Percentage from 'carbon-icons-svelte/lib/Percentage.svelte';
	import BatteryFull from 'carbon-icons-svelte/lib/BatteryFull.svelte';
	import UvIndexAlt from 'carbon-icons-svelte/lib/UvIndexAlt.svelte';
	import Light from 'carbon-icons-svelte/lib/Light.svelte';
	import TouchInteraction from 'carbon-icons-svelte/lib/TouchInteraction.svelte';
	import { InlineLoading } from 'carbon-components-svelte';
	import { env } from '$env/dynamic/public';
	import { onDestroy, onMount } from 'svelte';
	import { FlaggedEnum } from '$lib/utils/flagged-enum';
	import { ContextMenu, ContextMenuDivider, ContextMenuOption } from 'carbon-components-svelte';
	import type { Measurement } from '$lib/types';

	export let title: string;
	export let probeId: string;
	export let measurementType: number;
	export let refreshInterval: number;
	export let onEditClicked: (probeId: string) => void;

	let isLoading = true;
	let fetchError = '';
	let measurement: Measurement;
	let target: any;

	async function fetchMeasurements(ignoreLoading = false) {
		if (!ignoreLoading) {
			isLoading = true;
		}

		const options: RequestInit = {
			method: 'GET',
			headers: { 'Content-Type': 'application/json', accept: '*/*' }
		};
		measurement = await fetch(
			`${env.PUBLIC_API_URL}/measurement/lastMeasurement?probeId=` +
				probeId +
				`&measurementType=` +
				measurementType,
			options
		)
			.then((res) => res.json())
			.catch((err) => {
				fetchError = err.message;
			});

		isLoading = false;
	}

	let interval: number;
	onMount(() => {
		fetchMeasurements();

		if (refreshInterval > 0) {
			interval = setInterval(() => {
				fetchMeasurements(true);
			}, refreshInterval * 1000);
		}
	});

	onDestroy(() => {
		clearInterval(interval);
	});
</script>

<ContextMenu {target}>
	<ContextMenuOption indented labelText="Aktualisieren" on:click={() => fetchMeasurements(false)} />
	<ContextMenuOption
		indented
		labelText="Bearbeiten"
		on:click={() => {
			onEditClicked(probeId);
		}}
	/>
	<ContextMenuDivider />
	<ContextMenuOption indented kind="danger" labelText="Löschen" />
</ContextMenu>

<div bind:this={target}>
	<Tile class="dashboard-tile">
		<div class="flex-row-space">
			<h4>{title}</h4>
			{#if measurementType == 1}
				<TemperatureCelsius style="flex-end" size={32} />
			{:else if measurementType == 2}
				<Percentage style="flex-end" size={32} />
			{:else if measurementType == 4}
				<BatteryFull style="flex-end" size={32} />
			{:else if measurementType == 8}
				<HumidityAlt style="flex-end" size={32} />
			{:else if measurementType == 16}
				<UvIndexAlt style="flex-end" size={32} />
			{:else if measurementType == 32}
				<Light style="flex-end" size={32} />
			{:else if measurementType == 64}
				<SoilMoisture style="flex-end" size={32} />
			{:else if measurementType == 128}
				<TouchInteraction style="flex-end" size={32} />
			{:else if measurementType == 256}
				<CloudDownload style="flex-end" size={32} />
			{/if}
		</div>

		<h5 style="color:#C0C0C0;">Current</h5>
		{#if isLoading && measurement == null}
			<InlineLoading description="Lade Werte..." />
		{:else if fetchError !== ''}
			<p>{fetchError}</p>
		{:else}
			<div class="flex-row">
				<h2>{measurement.value}</h2>
				{#if measurementType == 1}
					<h4>°C</h4>
				{:else if measurementType == 2 || measurementType == 4 || measurementType == 8 || measurementType == 64}
					<h4>%</h4>
				{:else if measurementType == 32}
					<h4>LUX</h4>
				{:else if measurementType == 256}
					<h4>aPh</h4>
				{/if}
			</div>
		{/if}
	</Tile>
</div>

<style>
	:global(.dashboard-tile) {
		width: 20%;
		height: 20%;
	}
	.flex-row {
		display: flex;
		justify-content: flex-start;
	}
	.flex-row-space {
		display: flex;
		justify-content: space-between;
	}
</style>
