<script lang="ts">
	import 'carbon-components-svelte/css/all.css';

	import {
		Header,
		SideNav,
		SideNavItems,
		SideNavMenu,
		SideNavMenuItem,
		SideNavLink,
		SideNavDivider,
		SkipToContent,
		Content,
		Tile,
		Theme
	} from 'carbon-components-svelte';
	import { Home, DocumentTasks, CloudDataOps, Diagram } from 'carbon-icons-svelte';

	import '@carbon/styles/css/styles.css';
	import '@carbon/charts-svelte/styles.css';
	import { LineChart, ScaleTypes } from '@carbon/charts-svelte';

	import type { CarbonTheme } from 'carbon-components-svelte/types/Theme/Theme.svelte';

	import Grid, { GridItem, type LayoutChangeDetail } from 'svelte-grid-extended';
	import type { GridItemProps } from 'svelte-grid-extended/GridItem.svelte.js';

	let isSideNavOpen = false;
	let theme: CarbonTheme = 'g10';

	// @type {import('./$types').PageData}
	export let data;

	let items: GridItemProps[] = [{ id: '1', x: 0, y: 0, w: 2, h: 3, min: { w: 2, h: 3 } }];

	function logItem(event: CustomEvent<LayoutChangeDetail>) {
		console.log(event.detail);
		console.log(JSON.stringify(items));
		localStorage.setItem('layout', JSON.stringify(items));
	}

	if (typeof window !== 'undefined') {
		if (!localStorage.getItem('layout')) {
			localStorage.setItem('layout', JSON.stringify(items));
		} else {
			items = JSON.parse(localStorage.getItem('layout') as string);
		}
	}
</script>

<Theme bind:theme />

<Header company="SEVEN" platformName="Sandberg Electric Vehicle Eden Network" bind:isSideNavOpen>
	<!-- <svelte:fragment slot="skip-to-content">
		<SkipToContent />
	</svelte:fragment> -->
</Header>

<SideNav bind:isOpen={isSideNavOpen} rail>
	<SideNavItems>
		<SideNavLink icon={Home} text="Home" href="/" isSelected />
		<SideNavLink icon={DocumentTasks} text="Rovertasks" href="/" />
		<SideNavLink icon={CloudDataOps} text="Messdaten" href="/" />
		<SideNavDivider />
		<SideNavMenu icon={Diagram} text="Diagramme">
			<SideNavMenuItem href="/" text="Temperatur" />
		</SideNavMenu>
	</SideNavItems>
</SideNav>

<Content>
	<div class="full-height">
		<Grid cols={10} rows={0} itemSize={{ height: 100 }} on:change={logItem}>
			{#each items as { id, x, y, w, h, min } (id)}
				{#key id}
					<GridItem {id} bind:x bind:y bind:w bind:h bind:min>
						<Tile style="width: 100%; height: 100%">
							<LineChart
								data={data.measurements}
								options={{
									title: 'Measurements',
									height: '100%',
									width: '100%',
									data: { groupMapsTo: 'probeId' },
									curve: 'curveMonotoneX',
									points: { radius: 0 },
									axes: {
										left: { mapsTo: 'value', scaleType: ScaleTypes.LINEAR, title: 'Wert' },
										bottom: { mapsTo: 'time', scaleType: ScaleTypes.TIME, title: 'Datum' }
									},
									// @ts-ignore
									theme: theme
								}}
							/>
						</Tile>
					</GridItem>
				{/key}
			{/each}
		</Grid>
	</div>
</Content>

<style>
	.full-height {
		height: calc(100vh - 7rem);
	}
</style>
