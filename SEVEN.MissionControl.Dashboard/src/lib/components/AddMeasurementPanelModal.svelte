<script lang="ts">
	import type { AddMeasurementPanel } from '$lib/types';
	import { Modal, TextInput, ComboBox } from 'carbon-components-svelte';	

	let title = '';
	let probeId = '';
	let measurementType = 0;

	export let isOpen: boolean;
	export let onSubmitClicked: (panel: AddMeasurementPanel) => void;
</script>

<Modal
	bind:open={isOpen}
	modalHeading="Neues Panel"
	primaryButtonText="Panel erstellen"
	secondaryButtonText="Abbrechen"
	on:click:button--secondary={() => (isOpen = false)}
	on:open
	on:close
	on:submit={() => {
		onSubmitClicked({
			title,
			probeId,
			measurementType
		});

		isOpen = false;
		title = '';
		probeId = '';
		measurementType = 0;
	}}
>
	<TextInput id="panel-name" labelText="Panelname" placeholder="Panelname..." bind:value={title} />
	<TextInput id="probe-id" labelText="Probe" placeholder="Probe..." bind:value={probeId} />
	<TextInput
		id="probe-type"
		labelText="Probe-Typ"
		placeholder="Probe-Typ..."
		bind:value={measurementType}
	/>
	<ComboBox	
	titleText="Contact"
	placeholder="Select contact method"
	items={[
	  { id: 0, text: "Temperature" },
	  { id: 1, text: "Percent" },
	  { id: 2, text: "StateOfCharge" },
	  { id: 4, text: "Humidity" },
	  { id: 8, text: "UvRadiation" },
	  { id: 16, text: "LightIntensity" },
	  { id: 32, text: "SwitchingState" },
	  { id: 64, text: "SoilMoisture" },  
	]}
  />

</Modal>
