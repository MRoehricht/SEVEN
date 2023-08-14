<script lang="ts">
	import 'carbon-components-svelte/css/all.css';
	import '@carbon/styles/css/styles.css';
	import '@carbon/charts-svelte/styles.css';
	import Grid, { GridItem, type GridController } from 'svelte-grid-extended';
	import DashboardToolbar from '$lib/components/DashboardToolbar.svelte';
	import MeasurementPanel from '$lib/components/MeasurementPanel.svelte';
	import { Button, OverflowMenu, OverflowMenuItem } from 'carbon-components-svelte';
	import { Settings, Unlocked, Locked } from 'carbon-icons-svelte';
	import { panels } from '$lib/stores/dashboard-panel-store';
	import AddMeasurementPanelModal from '$lib/components/AddMeasurementPanelModal.svelte';
	import type { AddMeasurementPanel } from '$lib/types';

	let showAddPanelModal = false;
	let isLocked = true;
	let selectedPanel: AddMeasurementPanel | null = null;

	let needsUpdate: Date | null = null;

	const itemSize = { height: 100 };

	let gridController: GridController;

	function addNewPanel(panel: AddMeasurementPanel) {
		const w = 2;
		const h = 3;
		const newPosition = gridController.getFirstAvailablePosition(w, h);

		if (selectedPanel) {
			$panels = $panels.map((p) => {
				if (p.id === selectedPanel?.id) {
					return {
						...p,
						title: panel.title,
						probeId: panel.probeId,
						measurementType: panel.measurementType,
						refreshInterval: panel.refreshInterval
					};
				}
				return p;
			});
		} else {
			$panels = newPosition
				? [
						...$panels,
						{
							id: $panels.length + 1 + '',
							title: panel.title,
							probeId: panel.probeId,
							measurementType: panel.measurementType,
							refreshInterval: panel.refreshInterval,
							gridItem: { x: newPosition.x, y: newPosition.y, w, h, min: { w, h } }
						}
				  ]
				: $panels;
		}

		localStorage.setItem('dashboard-panels', JSON.stringify($panels));

		needsUpdate = new Date();
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

<AddMeasurementPanelModal
	bind:selectedPanel
	bind:isOpen={showAddPanelModal}
	onSubmitClicked={addNewPanel}
/>

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
				showAddPanelModal = true;
			}}
		/>
	</OverflowMenu>
</DashboardToolbar>

{#key needsUpdate}
	<div class="full-height">
		<Grid cols={10} rows={0} {itemSize} readOnly={isLocked} bind:controller={gridController}>
			{#each $panels as { id, title, probeId, refreshInterval, measurementType, gridItem } (id)}
				{#key id}
					<GridItem
						{id}
						bind:x={gridItem.x}
						bind:y={gridItem.y}
						bind:w={gridItem.w}
						bind:h={gridItem.h}
						bind:min={gridItem.min}
					>
						<MeasurementPanel
							{title}
							{probeId}
							{refreshInterval}
							{measurementType}
							onEditClicked={() => {
								const panel = $panels.find((p) => p.id === id);
								if (panel) {
									selectedPanel = {
										id: panel.id,
										title: panel.title,
										probeId: panel.probeId,
										measurementType: panel.measurementType,
										refreshInterval: panel.refreshInterval
									};
									showAddPanelModal = true;
								}
							}}
						/>
					</GridItem>
				{/key}
			{/each}
		</Grid>
	</div>
{/key}

<style>
	.full-height {
		height: calc(100vh - 7rem - 46px);
	}
</style>
