<script lang="ts">
	import 'carbon-components-svelte/css/all.css';
	import '@carbon/styles/css/styles.css';
	import '@carbon/charts-svelte/styles.css';
	import AddProbeModal from '$lib/components/AddProbeModal.svelte';
	import type { Probe } from '$lib/types';
	import { env } from '$env/dynamic/public';
	import DashboardToolbar from '$lib/components/DashboardToolbar.svelte';
	import { DataTable, OverflowMenu, OverflowMenuItem, DataTableSkeleton } from 'carbon-components-svelte';
	import { Button, Modal, TextInput } from 'carbon-components-svelte';
	import type { DataTableRow } from 'carbon-components-svelte/types/DataTable/DataTable.svelte';

	let showAddPanelModal = false;
	let selectedProbe:Probe;
	var selectedEditProbe: Probe | null = null;
	let openDelete = false;
	const headers = [
		{ key: 'name', value: 'Name' },
		{ key: 'lastContact', value: 'letzter kontakt' },
		{ key: 'overflow', empty: true }
	];

	let doFetchProbes = fetchProbes()

	function refreshData() {
		openDelete = false;
		selectedEditProbe = null;
		doFetchProbes = fetchProbes()
	}

	async function fetchProbes(): Promise<Probe[]> {
		const options: RequestInit = {
			method: 'GET',
			headers: { 'Content-Type': 'application/json', accept: '*/*' }			
		};
		return await fetch(`${env.PUBLIC_API_URL}/probe`, options).then((res) =>
			res.json()
		);
	}

	async function deleteProbe() {
		if(selectedProbe == null || selectedProbe.id.length == 0) return;
		const options: RequestInit = {
			method: 'DELETE',
			headers: { 'Content-Type': 'application/json', accept: '*/*' }			
		};
		await fetch(`${env.PUBLIC_API_URL}/probe?id=`+selectedProbe.id, options);		
		refreshData();
	}

	function setSelectedRow(rowDetail:DataTableRow){		
		selectedProbe = (<Probe>rowDetail);		
	}

	function openEditModal(){
		
		selectedEditProbe = selectedProbe;
		
		showAddPanelModal = true
	}	
</script>

<DashboardToolbar
	title="Sonden"
	crumbs={[
		{ label: 'Home', path: '/' },
		{ label: 'Sonden', path: '/probes' }
	]}
/>

{#await doFetchProbes}
<DataTableSkeleton showHeader={false} showToolbar={false} {headers} />
{:then rows}
	<DataTable sortable zebra {headers} {rows} on:mouseenter:row={(row) => setSelectedRow(row.detail)} >
		<svelte:fragment slot="cell" let:cell>
			{#if cell.key === 'overflow'}
				<OverflowMenu flipped>
					<OverflowMenuItem text="Bearbeiten" on:click={openEditModal} />					
					<OverflowMenuItem danger text="Löschen" on:click={() => openDelete = true} />
				</OverflowMenu>
			{:else}{cell.value}{/if}
		</svelte:fragment>
	</DataTable>
	<Button on:click={() => (showAddPanelModal = true)}>Sonde erstellen</Button>
{:catch error}
	<p>Error loading: {error.message}</p>
{/await}

<AddProbeModal bind:isOpen={showAddPanelModal} onSubmitClicked={refreshData} bind:selectedProbe={selectedEditProbe}/>

<Modal
  danger
  size="sm"
  bind:open={openDelete}
  modalHeading="Sonde löschen"
  primaryButtonText="Löschen"
  secondaryButtonText="Abbruch"
  on:click:button--secondary={() => (openDelete = false)}
  on:open
  on:close
  on:submit={deleteProbe}>
  <p>Damit wird die Sonde '{selectedProbe?.name}' unwiderruflich gelöscht.</p>
</Modal>