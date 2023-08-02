<script lang="ts">
	import { Breadcrumb, BreadcrumbItem, Button } from 'carbon-components-svelte';
	import type { BreadCrumbNavigationItem } from './DashboardToolbar';
	import { Settings, Unlocked, Locked } from 'carbon-icons-svelte';

	export let title: string;
	export let crumbs: BreadCrumbNavigationItem[];
	export let onSettingsClick: () => void;
	export let onLockToggle: (locked: boolean) => void;
	export let isLocked: boolean | undefined = false;
</script>

<div class="toolbar-container">
	<div>
		<Breadcrumb>
			{#each crumbs as crumb}
				<BreadcrumbItem href={crumb.path}>{crumb.label}</BreadcrumbItem>
			{/each}
		</Breadcrumb>
		<h4>{title}</h4>
	</div>
	<div>
		<Button
			icon={Settings}
			kind="ghost"
			tooltipPosition="left"
			iconDescription="Einstellungen"
			on:click={onSettingsClick}
		/>
		{#if isLocked}
			<Button
				icon={Unlocked}
				kind="ghost"
				tooltipPosition="left"
				iconDescription="Entsperren"
				on:click={() => onLockToggle(false)}
			/>
		{:else}
			<Button
				icon={Locked}
				kind="ghost"
				tooltipPosition="left"
				iconDescription="Sperren"
				on:click={() => onLockToggle(true)}
			/>
		{/if}
	</div>
</div>

<style>
	.toolbar-container {
		display: flex;
		flex-direction: row;
		justify-content: space-between;
		align-items: center;
	}
</style>
