<script lang="ts">
	import 'carbon-components-svelte/css/all.css';
	import '@carbon/styles/css/styles.css';
	import '@carbon/charts-svelte/styles.css';
	import type { Probe } from '$lib/types';
	import DashboardToolbar from '$lib/components/DashboardToolbar.svelte';
	import DeviceQueryLanguageBuilder from '$lib/components/DeviceQueryLanguageBuilder.svelte';
</script>

<DashboardToolbar
	title="DQL-Inspector"
	crumbs={[
		{ label: 'Home', path: '/' },
		{ label: 'DQL-Inspector', path: '/dql-inspector' }
	]}
/>

<DeviceQueryLanguageBuilder />
