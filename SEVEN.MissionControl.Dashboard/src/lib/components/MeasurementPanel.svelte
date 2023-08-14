<script lang="ts">
	import { Breadcrumb, BreadcrumbItem, Tile } from 'carbon-components-svelte';
	import type { Measurement } from '$lib/types';
	import { LineChart, ScaleTypes } from '@carbon/charts-svelte';
	import { ToolbarControlTypes } from '@carbon/charts';
	import { env } from '$env/dynamic/public';

	export let title: string;
	export let probeId: string;
	export let measurementType: number;

	async function fetchMeasurements(): Promise<Measurement[]> {
		const options: RequestInit = {
			method: 'POST',
			headers: { 'Content-Type': 'application/json', accept: '*/*' },
			body: JSON.stringify({
				probeId: probeId,
				type: Number(measurementType)
			})
		};
		return await fetch(`${env.PUBLIC_API_URL}/measurement/filter`, options).then((res) =>
			res.json()
		);
	}
</script>

<Tile class="dashboard-tile">
	{#await fetchMeasurements()}
		<LineChart
			data={[]}
			options={{
				title,
				height: '100%',
				width: '100%',
				data: { loading: true }
			}}
		/>
	{:then measurements}
		<LineChart
			data={measurements}
			options={{
				title,
				height: '100%',
				width: '100%',
				data: { groupMapsTo: 'probeId' },
				curve: 'curveMonotoneX',
				points: { radius: 0 },
				axes: {
					left: { mapsTo: 'value', scaleType: ScaleTypes.LINEAR, title: 'Wert' },
					bottom: { mapsTo: 'time', scaleType: ScaleTypes.TIME, title: 'Datum' }
				},
				toolbar: {
					enabled: true,
					controls: [
						{
							type: ToolbarControlTypes.CUSTOM,
							title: 'Probe',
							clickFunction: () => {
								alert('clicked');
							}
						}
					]
				}
			}}
		/>
	{:catch error}
		<p>{error.message}</p>
	{/await}
</Tile>

<style>
	:global(.dashboard-tile) {
		width: 100%;
		height: 100%;
	}
</style>
