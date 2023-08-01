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
	import DashboardToolbar from '$lib/components/DashboardToolbar.svelte';
	import MeasurementPanel from '$lib/components/MeasurementPanel.svelte';

	let isSideNavOpen = false;
	let theme: CarbonTheme = 'g10';
	let isLocked = true;

	// @type {import('./$types').PageData}
	export let data;

	let items: GridItemProps[] = [{ id: '1', x: 0, y: 0, w: 2, h: 3, min: { w: 2, h: 3 } }];

	function logItem(event: CustomEvent<LayoutChangeDetail>) {
		console.log(event.detail);
	}

	if (typeof window !== 'undefined') {
		if (!localStorage.getItem('layout')) {
			localStorage.setItem('layout', JSON.stringify(items));
		} else {
			items = JSON.parse(localStorage.getItem('layout') as string);
		}
	}

	function toggleDashboardLock(locked: boolean) {
		isLocked = locked;

		if (locked) {
			localStorage.setItem('layout', JSON.stringify(items));
		}
	}
</script>

<DashboardToolbar
	title="Measurements"
	crumbs={[
		{ label: 'Home', path: '/' },
		{ label: 'Measurements', path: '/measurements' }
	]}
	onSettingsClick={() => alert('Settings clicked')}
	onLockToggle={toggleDashboardLock}
	{isLocked}
/>
<div class="full-height">
	<Grid cols={10} rows={0} itemSize={{ height: 100 }} on:change={logItem} readOnly={isLocked}>
		{#each items as { id, x, y, w, h, min } (id)}
			{#key id}
				<GridItem {id} bind:x bind:y bind:w bind:h bind:min>
					<MeasurementPanel measurements={data.measurements} />
				</GridItem>
			{/key}
		{/each}
	</Grid>
</div>

<style>
	.full-height {
		height: calc(100vh - 7rem - 46px);
	}
</style>
