<script lang="ts">
	import { Breadcrumb, BreadcrumbItem, Tile } from 'carbon-components-svelte';
	import type { BreadCrumbNavigationItem } from './DashboardToolbar';
	import type { Measurement } from '../../routes/measurements/+page';
	import { LineChart, ScaleTypes } from '@carbon/charts-svelte';
	import { ToolbarControlTypes } from '@carbon/charts';

	export let measurements: Measurement[];
</script>

<Tile style="width: 100%; height: 100%">
	<LineChart
		data={measurements}
		options={{
			title: 'Probe1',
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
