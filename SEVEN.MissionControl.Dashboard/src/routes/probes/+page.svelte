<script lang="ts">
	import 'carbon-components-svelte/css/all.css';
	import '@carbon/styles/css/styles.css';
	import '@carbon/charts-svelte/styles.css';
	import AddProbeModal from '$lib/components/AddProbeModal.svelte';
	import type { Probe } from '$lib/types';
	import { env } from '$env/dynamic/public';
	import DashboardToolbar from '$lib/components/DashboardToolbar.svelte';
	import {
		DataTable,
		OverflowMenu,
		OverflowMenuItem,
		DataTableSkeleton,
		Toolbar,
		ToolbarContent,
		ToolbarMenu,
		ToolbarMenuItem,
		ToolbarSearch
	} from 'carbon-components-svelte';
	import { Button, Modal, TextInput } from 'carbon-components-svelte';
	import type { DataTableRow } from 'carbon-components-svelte/types/DataTable/DataTable.svelte';
	import { onMount } from 'svelte';

	let isLoading = true;
	let fetchError = '';
	let probes: Probe[] = [];
	let selectedProbe: Probe | null = null;
	let showAddPanelModal = false;
	let showDeleteModal = false;

	const headers = [
		{ key: 'id', value: 'ID' },
		{ key: 'name', value: 'Name' },
		{ key: 'lastContact', value: 'letzter kontakt' },
		{ key: 'overflow', empty: true }
	];

	onMount(() => {
		fetchProbes();
	});

	async function fetchProbes() {
		isLoading = true;

		const options: RequestInit = {
			method: 'GET',
			headers: { 'Content-Type': 'application/json', accept: '*/*' }
		};
		probes = await fetch(`${env.PUBLIC_API_URL}/probe`, options).then((res) => res.json());

		isLoading = false;
	}

	async function deleteProbe() {
		if (selectedProbe == null || selectedProbe.id.length == 0) return;

		const options: RequestInit = {
			method: 'DELETE',
			headers: { 'Content-Type': 'application/json', accept: '*/*' }
		};

		await fetch(`${env.PUBLIC_API_URL}/probe?id=` + selectedProbe.id, options);

		showDeleteModal = false;

		fetchProbes();
	}

	function setSelectedProbe(probe: DataTableRow) {
		selectedProbe = <Probe>probe;
	}
</script>

<DashboardToolbar
	title="Sonden"
	crumbs={[
		{ label: 'Home', path: '/' },
		{ label: 'Sonden', path: '/probes' }
	]}
/>

{#if isLoading}
	<DataTableSkeleton showHeader={false} showToolbar={false} {headers} />
{:else if fetchError !== ''}
	<p>{fetchError}</p>
{:else}
	<DataTable sortable zebra {headers} bind:rows={probes}>
		<Toolbar>
			<ToolbarContent>
				<ToolbarSearch />
				<Button
					on:click={() => {
						selectedProbe = null;
						showAddPanelModal = true;
					}}>Sonde erstellen</Button
				>
			</ToolbarContent>
		</Toolbar>
		<svelte:fragment slot="cell" let:cell let:row>
			{#if cell.key === 'overflow'}
				<OverflowMenu flipped>
					<OverflowMenuItem
						text="Bearbeiten"
						on:click={() => {
							setSelectedProbe(row);
							showAddPanelModal = true;
						}}
					/>
					<OverflowMenuItem
						danger
						text="Löschen"
						on:click={() => {
							setSelectedProbe(row);
							showDeleteModal = true;
						}}
					/>
				</OverflowMenu>
			{:else}{cell.value}{/if}
		</svelte:fragment>
	</DataTable>
{/if}

<AddProbeModal
	bind:isOpen={showAddPanelModal}
	bind:selectedProbe
	onSubmitClicked={() => {
		selectedProbe = null;
		fetchProbes();
	}}
/>

<Modal
	danger
	size="sm"
	bind:open={showDeleteModal}
	modalHeading="Sonde löschen"
	primaryButtonText="Löschen"
	secondaryButtonText="Abbruch"
	on:click:button--secondary={() => (showDeleteModal = false)}
	on:open
	on:close
	on:submit={deleteProbe}
>
	<p>Damit wird die Sonde '{selectedProbe?.name}' unwiderruflich gelöscht.</p>
</Modal>
