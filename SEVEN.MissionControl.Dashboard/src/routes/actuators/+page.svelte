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
</script>

<DashboardToolbar
	title="Aktoren"
	crumbs={[
		{ label: 'Home', path: '/' },
		{ label: 'Aktoren', path: '/actuators' }
	]}
/>
