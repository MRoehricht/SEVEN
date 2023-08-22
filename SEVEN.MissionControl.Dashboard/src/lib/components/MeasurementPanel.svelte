<script lang="ts">
	import { Tile } from 'carbon-components-svelte';
	import { measurementTypeLabels, type Measurement, type MeasurementWithGroup } from '$lib/types';
	import { LineChart, ScaleTypes, type LineChartOptions } from '@carbon/charts-svelte';
	import { ToolbarControlTypes } from '@carbon/charts';
	import { env } from '$env/dynamic/public';
	import { onDestroy, onMount } from 'svelte';
	import { FlaggedEnum } from '$lib/utils/flagged-enum';
	import CurrentMeasurementPanel from '$lib/components/CurrentMeasurementPanel.svelte';

	export let title: string;
	export let probeId: string;
	export let refreshInterval: number;
	export let measurementType: number;
	export let showCurrentValue: boolean = false;
	export let onEditClicked: (probeId: string) => void;

	const flags = new FlaggedEnum(measurementTypeLabels);

	let isLoading = true;
	let fetchError = '';
	let lastUpdated = new Date();
	let measurements: MeasurementWithGroup[] = [];
	let reduceData = true;
	let reduceDataText = '';

	function toggleReduceData() {
		reduceData = !reduceData;
		fetchMeasurements();
	}

	async function fetchMeasurements(ignoreLoading = false) {
		if (!ignoreLoading) {
			isLoading = true;
		}

		const options: RequestInit = {
			method: 'POST',
			headers: { 'Content-Type': 'application/json', accept: '*/*' },
			body: JSON.stringify({
				probeId: probeId,
				type: Number(measurementType),
				ReduceData: reduceData
			})
		};
		measurements = await fetch(`${env.PUBLIC_API_URL}/measurement/filter`, options)
			.then((res) => res.json())
			.catch((err) => {
				fetchError = err.message;
			});

		measurements = measurements.map((m) => {
			return {
				...m,
				group: flags.getLabelFromValue(m.measurementType)
			};
		});

		// force redraw
		measurements = [...measurements];

		lastUpdated = new Date();
		isLoading = false;
	}

	function getLineChartOptions(isLoading: boolean): LineChartOptions {
		return {
			title,
			height: '100%',
			width: '100%',
			data: { loading: isLoading },
			curve: 'curveMonotoneX',
			points: { radius: 0 },
			axes: {
				left: { mapsTo: 'value', scaleType: ScaleTypes.LINEAR, title: 'Wert' },
				bottom: { mapsTo: 'time', scaleType: ScaleTypes.TIME, title: 'Datum' }
			},
			timeScale: {
				addSpaceOnEdges: 0
			},
			toolbar: {
				enabled: true,
				controls: [
					{
						type: ToolbarControlTypes.CUSTOM,
						title: 'Bearbeiten',
						text: 'Bearbeiten',
						clickFunction: () => {
							onEditClicked(probeId);
						}
					},
					{
						type: ToolbarControlTypes.CUSTOM,
						title: 'Aktualisieren',
						text: 'Aktualisieren',
						clickFunction: () => {
							fetchMeasurements();
						}
					},
					{
						type: ToolbarControlTypes.CUSTOM,
						title: reduceDataText,
						text: reduceDataText,
						clickFunction: () => {
							toggleReduceData();
						}
					}
				]
			}
		};
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

	$: {
		reduceDataText = reduceData ? 'Alle Daten anzeigen' : 'Daten reduzieren';
	}
</script>

{#if !showCurrentValue}
	<Tile class="dashboard-tile">
		{#if isLoading}
			<LineChart data={[]} options={getLineChartOptions(true)} />
		{:else if fetchError !== ''}
			<p>{fetchError}</p>
		{:else}
			<div class="flex">
				<LineChart data={measurements} options={getLineChartOptions(false)} />
				{#if refreshInterval > 0}
					<span class="last-updated">
						aktualisiert: {lastUpdated.toLocaleTimeString()}
					</span>
				{/if}
			</div>
		{/if}
	</Tile>
{:else}
	<CurrentMeasurementPanel {probeId} {measurementType} {title} {refreshInterval} {onEditClicked} />
{/if}

<style>
	:global(.dashboard-tile) {
		width: 100%;
		height: 100%;
	}

	.flex {
		display: flex;
		flex-direction: column;
		height: 100%;
	}

	.last-updated {
		font-size: 0.75rem;
		display: flex;
		align-items: end;
		justify-content: end;
		width: 100%;
	}
</style>
