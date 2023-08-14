<script lang="ts">
	import { Tile } from 'carbon-components-svelte';
	import type { Measurement } from '$lib/types';
	import { LineChart, ScaleTypes, type LineChartOptions } from '@carbon/charts-svelte';
	import { ToolbarControlTypes } from '@carbon/charts';
	import { env } from '$env/dynamic/public';
	import { onMount } from 'svelte';

	export let title: string;
	export let probeId: string;
	export let measurementType: number;

	let isLoading = true;
	let fetchError = '';
	let measurements: Measurement[] = [];

	async function fetchMeasurements() {
		isLoading = true;
		const options: RequestInit = {
			method: 'POST',
			headers: { 'Content-Type': 'application/json', accept: '*/*' },
			body: JSON.stringify({
				probeId: probeId,
				type: Number(measurementType)
			})
		};
		measurements = await fetch(`${env.PUBLIC_API_URL}/measurement/filter`, options)
			.then((res) => res.json())
			.catch((err) => {
				fetchError = err.message;
			});

		isLoading = false;
	}

	function getLineChartOptions(isLoading: boolean): LineChartOptions {
		return {
			title,
			height: '100%',
			width: '100%',
			data: { groupMapsTo: 'probeId', loading: isLoading },
			curve: 'curveMonotoneX',
			points: { radius: 0 },
			axes: {
				left: { mapsTo: 'value', scaleType: ScaleTypes.LINEAR, title: 'Wert' },
				bottom: { mapsTo: 'time', scaleType: ScaleTypes.TIME, title: 'Datum' }
			},
			zoomBar: {
				top: {
					enabled: true
				}
			},
			toolbar: {
				enabled: true,
				controls: [
					{
						type: ToolbarControlTypes.CUSTOM,
						title: 'Aktualisieren',
						text: 'Aktualisieren',
						clickFunction: () => {
							fetchMeasurements();
						}
					}
				]
			}
		};
	}

	onMount(() => {
		fetchMeasurements();
	});
</script>

<Tile class="dashboard-tile">
	{#if isLoading}
		<LineChart data={[]} options={getLineChartOptions(true)} />
	{:else if fetchError !== ''}
		<p>{fetchError}</p>
	{:else}
		<LineChart data={measurements} options={getLineChartOptions(false)} />
	{/if}
</Tile>

<style>
	:global(.dashboard-tile) {
		width: 100%;
		height: 100%;
	}
</style>
