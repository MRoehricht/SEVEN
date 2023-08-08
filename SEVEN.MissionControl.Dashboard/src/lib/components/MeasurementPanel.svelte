<script lang="ts">
	import { Breadcrumb, BreadcrumbItem, Tile } from 'carbon-components-svelte';
	import type { Measurement } from '../../routes/+page.server';
	import { LineChart, ScaleTypes } from '@carbon/charts-svelte';
	import { ToolbarControlTypes } from '@carbon/charts';

	export let title: string;
	export let measurements: Measurement[];
</script>

<Tile class="dashboard-tile">
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
</Tile>

<style>
	:global(.dashboard-tile) {
		width: 100%;
		height: 100%;
	}
</style>
