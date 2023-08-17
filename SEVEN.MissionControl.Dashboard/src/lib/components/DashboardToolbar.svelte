<script lang="ts">
	import { Breadcrumb, BreadcrumbItem, Button } from 'carbon-components-svelte';
	import type { BreadCrumbNavigationItem } from './DashboardToolbar';

	export let title: string;
	export let crumbs: BreadCrumbNavigationItem[];
</script>

<div class="toolbar-container">
	<div>
		<Breadcrumb>
			{#each crumbs as crumb}
				<BreadcrumbItem href={crumb.path}>{crumb.label}</BreadcrumbItem>
			{/each}
		</Breadcrumb>
		<h4 class="toolbar-heading">{title}</h4>
	</div>
	<div class="toolbar-actions">
		<slot />
	</div>
</div>

<style>
	.toolbar-container {
		display: flex;
		flex-direction: row;
		justify-content: space-between;
		align-items: center;
	}

	.toolbar-actions {
		display: flex;
		flex-direction: row;
		justify-content: space-between;
		align-items: center;
	}

	.toolbar-heading {
		margin-top: 6px;
	}
</style>
