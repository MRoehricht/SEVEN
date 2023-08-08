<script lang="ts">
	import 'carbon-components-svelte/css/all.css';
	import '@carbon/styles/css/styles.css';
	import '@carbon/charts-svelte/styles.css';
	import Grid, {
		GridItem,
		type GridController,
		type LayoutChangeDetail
	} from 'svelte-grid-extended';
	import DashboardToolbar from '$lib/components/DashboardToolbar.svelte';
	import MeasurementPanel from '$lib/components/MeasurementPanel.svelte';
	import {
		Button,
		Modal,
		OverflowMenu,
		OverflowMenuItem,
		TextInput
	} from 'carbon-components-svelte';
	import { Settings, Unlocked, Locked } from 'carbon-icons-svelte';
	import { panels } from '$lib/stores/dashboard-panel-store';

	let isLocked = true;
	let addPanelOpen = false;
	let addPanelName = '';
	const itemSize = { height: 100 };

	// @type {import('./$types').PageData}
	export let data;

	let gridController: GridController;

	function addNewItem(title: string) {
		const w = 2;
		const h = 3;
		const newPosition = gridController.getFirstAvailablePosition(w, h);

		$panels = newPosition
			? [
					...$panels,
					{
						id: $panels.length + 1 + '',
						title,
						gridItem: { x: newPosition.x, y: newPosition.y, w, h, min: { w, h } }
					}
			  ]
			: $panels;
	}

	if (typeof window !== 'undefined') {
		if (!localStorage.getItem('dashboard-panels')) {
			localStorage.setItem('dashboard-panels', JSON.stringify($panels));
		} else {
			$panels = JSON.parse(localStorage.getItem('dashboard-panels') as string);
		}
	}

	function toggleDashboardLock(locked: boolean) {
		isLocked = locked;

		if (locked) {
			localStorage.setItem('dashboard-panels', JSON.stringify($panels));
		}
	}
</script>

<Modal
	bind:open={addPanelOpen}
	modalHeading="Neues Panel"
	primaryButtonText="Panel erstellen"
	secondaryButtonText="Abbrechen"
	on:click:button--secondary={() => (addPanelOpen = false)}
	on:open
	on:close
	on:submit={() => {
		addNewItem(addPanelName);
		addPanelOpen = false;
		addPanelName = '';
	}}
>
	<TextInput
		id="panel-name"
		labelText="Panelname"
		placeholder="Panelname..."
		bind:value={addPanelName}
	/>
</Modal>

<DashboardToolbar title="Mein Dashboard" crumbs={[{ label: 'Dashboard', path: '/' }]}>
	{#if isLocked}
		<Button
			icon={Unlocked}
			kind="ghost"
			tooltipPosition="left"
			iconDescription="Entsperren"
			on:click={() => toggleDashboardLock(false)}
		/>
	{:else}
		<Button
			icon={Locked}
			kind="danger-ghost"
			tooltipPosition="left"
			iconDescription="Sperren"
			on:click={() => toggleDashboardLock(true)}
		/>
	{/if}
	<OverflowMenu icon={Settings} size="xl" flipped iconDescription="Einstellungen">
		<OverflowMenuItem
			text="Neues Panel"
			on:click={() => {
				addPanelOpen = true;
			}}
		/>
	</OverflowMenu>
</DashboardToolbar>

<div class="full-height">
	<Grid cols={10} rows={0} {itemSize} readOnly={isLocked} bind:controller={gridController}>
		{#each $panels as { id, title, gridItem } (id)}
			{#key id}
				<GridItem
					{id}
					bind:x={gridItem.x}
					bind:y={gridItem.y}
					bind:w={gridItem.w}
					bind:h={gridItem.h}
					bind:min={gridItem.min}
				>
					<MeasurementPanel {title} measurements={data.measurements} />
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
