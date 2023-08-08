<script lang="ts">
	import 'carbon-components-svelte/css/all.css';
	import '@carbon/styles/css/styles.css';
	import '@carbon/charts-svelte/styles.css';

	import type { CarbonTheme } from 'carbon-components-svelte/types/Theme/Theme.svelte';

	import type { GridItemProps } from 'svelte-grid-extended/GridItem.svelte.js';
	import DashboardToolbar from '$lib/components/DashboardToolbar.svelte';
	import MeasurementPanel from '$lib/components/MeasurementPanel.svelte';

	let theme: CarbonTheme = 'g10';

	import { DataTable, OverflowMenu, OverflowMenuItem, Tile } from 'carbon-components-svelte';

	import { Button, Modal, TextInput } from 'carbon-components-svelte';

	import copy from 'clipboard-copy';
	import { CopyButton } from 'carbon-components-svelte';
	import { Label } from 'carbon-icons-svelte';

	let open = false;

	const headers = [
		{ key: 'name', value: 'Name' },
		{ key: 'lastContact', value: 'letzter kontakt' },
		{ key: 'overflow', empty: true }
	];

	const rows = [
		{ id: 'a', name: 'Probe1', lastContact: '04.08.2023 01:08:34' },
		{ id: 'b', name: 'Katzengras', lastContact: '06.08.2023 14:55:12' },
		{ id: 'c', name: 'Kartoffeln', lastContact: '07.08.2023 08:22:15' }
	];
</script>

<DashboardToolbar
	title="Sonden"
	crumbs={[
		{ label: 'Home', path: '/' },
		{ label: 'Sonden', path: '/probes' }
	]}
/>

<DataTable sortable zebra {headers} {rows}>
	<svelte:fragment slot="cell" let:cell>
		{#if cell.key === 'overflow'}
			<OverflowMenu flipped>
				<OverflowMenuItem text="Bearbeiten" on:click={() => (open = true)} />
				<OverflowMenuItem
					href="https://cloud.ibm.com/docs/loadbalancer-service"
					text="API documentation"
				/>
				<OverflowMenuItem danger text="Löschen" />
			</OverflowMenu>
		{:else}{cell.value}{/if}
	</svelte:fragment>
</DataTable>

<Button on:click={() => (open = true)}>Sonde erstellen</Button>

<Modal
	preventCloseOnClickOutside
	bind:open
	modalHeading="Sonde"
	primaryButtonText="Speichern"
	secondaryButtonText="Abbrechen"
	selectorPrimaryFocus="#probe-name"
	on:click:button--secondary={() => (open = false)}
	on:open
	on:close
	on:submit
>
	<p>Hier findest Du alle Eigenschaften deiner Sonde.</p>
	<br />
	<TextInput id="probe-name" labelText="Sondenname" placeholder="Enter probe name..." />
	<TextInput id="probe-name" labelText="Sondenname" placeholder="Enter probe name..." />

	<Tile light
		>API-Key
		<CopyButton text="APIKEY" copy={(text) => copy(text)} />
	</Tile>
</Modal>
