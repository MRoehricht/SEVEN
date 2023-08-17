<script lang="ts">
	import 'carbon-components-svelte/css/all.css';
	import '@carbon/styles/css/styles.css';
	import '@carbon/charts-svelte/styles.css';
	import type { Probe } from '$lib/types';
	import DashboardToolbar from '$lib/components/DashboardToolbar.svelte';
	import DeviceQueryLanguageBuilder from '$lib/components/DeviceQueryLanguageBuilder.svelte';

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

<DeviceQueryLanguageBuilder />
