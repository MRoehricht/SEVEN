<script lang="ts">
	import 'carbon-components-svelte/css/all.css';
	import '@carbon/styles/css/styles.css';
	import '@carbon/charts-svelte/styles.css';
	import type { Probe } from '$lib/types';
	import DashboardToolbar from '$lib/components/DashboardToolbar.svelte';
</script>

<DashboardToolbar
	title="Aktoren"
	crumbs={[
		{ label: 'Home', path: '/' },
		{ label: 'Aktoren', path: '/actuators' }
	]}
/>
