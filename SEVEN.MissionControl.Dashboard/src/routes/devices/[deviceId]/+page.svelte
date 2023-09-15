<script lang="ts">
	import { page } from '$app/stores';
	import {
		FileUploader,
		TextInput,
		MultiSelect,
		Button,
		FileUploaderSkeleton,
		InlineLoading,
		TextInputSkeleton
	} from 'carbon-components-svelte';
	import { measurementTypeLabels, type Probe, LoadingStatus } from '$lib/types';
	import { DeviceService } from '$lib/utils/deviceService';
	import { FlaggedEnum } from '$lib/utils/flagged-enum';
	import Undo from 'carbon-icons-svelte/lib/Undo.svelte';
	import { onDestroy } from 'svelte';
	import DashboardToolbar from '$lib/components/DashboardToolbar.svelte';

	let saveButtonWidth: number;
	$: deviceId = $page.params.deviceId;
	$: deviceId && onChange();
	let name = '';
	const flags = new FlaggedEnum(measurementTypeLabels);
	const flagsWithLabels = flags.getAllValuesAsIdTextObjects();
	let selectedIds: string[] = [];
	let multiSelectLabel = '';
	let files: File[] = [];
	let saveStatus: LoadingStatus = LoadingStatus.dormant;
	let loadStatus: LoadingStatus = LoadingStatus.dormant;
	let timeout: number;
	var probe: Probe | null;

	const descriptionMap = new Map<LoadingStatus, string>([
		[LoadingStatus.active, 'Speicher...'],
		[LoadingStatus.finished, 'Erfolgreich']
	]);

	$: {
		multiSelectLabel = flags.getLabelsFromIds(selectedIds).join(', ') || 'Wähle Messwerte';
	}

	async function onChange(showStatus: boolean = false) {
		if (showStatus) loadStatus = LoadingStatus.active;
		probe = await DeviceService.fetchDevice(deviceId);
		if (probe != null) {
			name = probe.name;
			selectedIds = flags.getValuesAsStrings(probe.measurementsType);
			if (showStatus) loadStatus = LoadingStatus.finished;
		} else {
			if (showStatus) loadStatus = LoadingStatus.error;
		}
		timeout = setTimeout(() => {
			if (showStatus) loadStatus = LoadingStatus.dormant;
		}, 500);
	}

	async function saveDevice() {
		if (probe == null) return;
		saveStatus = LoadingStatus.active;
		probe = await DeviceService.updateDevice(probe.id, name, selectedIds);
		saveStatus = LoadingStatus.finished;
		timeout = setTimeout(() => {
			saveStatus = LoadingStatus.dormant;
		}, 1000);
	}

	onDestroy(() => clearTimeout(timeout));
</script>

{#if probe != null}
	<DashboardToolbar
		title="Geräteeinstellungen"
		crumbs={[
			{ label: 'Home', path: '/' },
			{ label: 'Geräte', path: '/devices' },
			{ label: probe.name, path: '/devices/' + probe.id }
		]}
	/>

	<TextInput id="probe-name" labelText="Sondenname" placeholder="Sondenname..." bind:value={name} />
	<MultiSelect
		titleText="Messwerte"
		label={multiSelectLabel}
		items={flagsWithLabels}
		bind:selectedIds
		sortItem={() => {}}
	/>
	<div class="button-row">
		{#if saveStatus !== LoadingStatus.dormant}
			<Button kind="secondary" icon={Undo} disabled />
			<div class="save-button" style="width: {saveButtonWidth}px;">
				<InlineLoading status={saveStatus} description={descriptionMap.get(saveStatus)} />
			</div>
		{:else if loadStatus !== LoadingStatus.dormant}
			<div class="save-button">
				<InlineLoading status={loadStatus} />
			</div>
			<Button disabled>Speichern</Button>
		{:else}
			<Button
				kind="secondary"
				icon={Undo}
				iconDescription="Neu laden"
				on:click={async () => {
					await onChange(true);
				}}
			/>
			<div bind:clientWidth={saveButtonWidth}>
				<Button
					on:click={async () => {
						await saveDevice();
					}}>Speichern</Button
				>
			</div>
		{/if}
	</div>

	<br /><br />
	<FileUploader
		multiple
		labelTitle="Konfigurationsdatei hochladen"
		buttonLabel="Hochladen"
		labelDescription="Nur TOML Dateien sind erlaubt."
		accept={['.toml']}
		status="complete"
		{files}
	/>
{:else}
	<DashboardToolbar
		title="Geräteeinstellungen"
		crumbs={[
			{ label: 'Home', path: '/' },
			{ label: 'Geräte', path: '/devices' }
		]}
	/>
	<TextInputSkeleton />
	<TextInputSkeleton />
	<div class="button-row">
		<Button skeleton icon={Undo} />
		<Button skeleton />
	</div>
	<br /><br />
	<FileUploaderSkeleton />
{/if}

<style>
	.button-row {
		display: flex;
		justify-content: flex-end;
		vertical-align: middle;
	}

	.save-button {
		display: flex;
		align-content: center;
		vertical-align: middle;
	}
</style>
