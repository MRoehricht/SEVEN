<script lang="ts">
	import type { DqlProperty, DqlToken } from '$lib/types';
	import { Button, Popover } from 'carbon-components-svelte';
	import { Play } from 'carbon-icons-svelte';
	import { v4 as uuidv4 } from 'uuid';

	type DqlTokenGroup = {
		type: 'form' | 'logical';
		tokens: DqlToken[];
	};

	let whereTokens: DqlTokenGroup[] = [];
	let suggestions: string[] = [];
	let selectedToken: string | null = null;
	let needsRerender: Date = new Date();

	let operators = ['==', '!=', '>', '<', '>=', '<='];
	let logicals = ['AND', 'OR'];

	export let dqlProperties: DqlProperty[];
	export let onExecuteQuery: (tokens: DqlToken[]) => void;

	function addDefaultWhereTokens() {
		let newTokens = [] as DqlTokenGroup[];
		const defaultTokens = {
			type: 'form',
			tokens: [
				{
					id: uuidv4(),
					type: 'property',
					value: dqlProperties[0].propertyName,
					index: 0
				},
				{
					id: uuidv4(),
					type: 'operator',
					value: operators[0],
					index: 0
				},
				{
					id: uuidv4(),
					type: 'value',
					value: 'placeholder',
					index: 0
				}
			]
		} as DqlTokenGroup;

		if (whereTokens.length === 0) {
			newTokens.push(defaultTokens);
		} else {
			newTokens.push({
				type: 'logical',
				tokens: [
					{
						id: uuidv4(),
						type: 'operator',
						value: 'AND',
						index: 0
					}
				]
			});
			newTokens.push(defaultTokens);
		}

		whereTokens = [...whereTokens, ...newTokens];
	}

	function showPropertySuggestions(tokenId: string) {
		suggestions = dqlProperties.map((p) => p.propertyName).flat();
		selectedToken = tokenId;
	}

	function showOperatorSuggestions(tokenId: string) {
		suggestions = operators;
		selectedToken = tokenId;
	}

	function showLogicalSuggestions(tokenId: string) {
		suggestions = logicals;
		selectedToken = tokenId;
	}

	function setCustomSuggestions(newSuggestions: string[]) {
		suggestions = newSuggestions;
	}

	function updateTokenValue(tokenId: string, value: string) {
		console.log(tokenId, value);

		let token = whereTokens
			.map((t) => t.tokens)
			.flat()
			.find((t) => t.id === tokenId);

		if (token) {
			token.value = value;
			suggestions = [];
			needsRerender = new Date();
		}
	}
</script>

<div>
	<div class="field-row">
		<span class="bx--tile field-row-label">FROM</span>
		<button class="bx--tile field-row-button">measurements</button>
		<span class="bx--tile field-row-label">WHERE</span>
		{#key needsRerender}
			{#each whereTokens as token}
				{#if token.type == 'logical'}
					<div style:position="relative">
						<button
							class="bx--tile field-row-button"
							on:click={() => showLogicalSuggestions(token.tokens[0].id)}
							>{token.tokens[0].value}</button
						>
						<Popover open={selectedToken === token.tokens[0].id} align="bottom-left">
							{#each suggestions as suggestion}
								<div
									class="popover-item"
									role="button"
									tabindex="0"
									on:click={() => {
										updateTokenValue(token.tokens[0].id, suggestion);
										selectedToken = null;
									}}
									on:keydown={(e) => {
										if (e.key === 'Enter') {
											updateTokenValue(token.tokens[0].id, suggestion);
											selectedToken = null;
										}
									}}
								>
									{suggestion}
								</div>
							{/each}
						</Popover>
					</div>
				{:else if token.type == 'form'}
					<div class="field-row-form">
						{#each token.tokens as subToken}
							{#if subToken.type == 'property'}
								<div style:position="relative">
									<button
										class="bx--tile field-row-button"
										on:click={() => showPropertySuggestions(subToken.id)}>{subToken.value}</button
									>
									<Popover open={selectedToken === subToken.id} align="bottom-left">
										{#each suggestions as suggestion}
											<div
												class="popover-item"
												role="button"
												tabindex="0"
												on:click={() => {
													updateTokenValue(subToken.id, suggestion);
													selectedToken = null;
												}}
												on:keydown={(e) => {
													if (e.key === 'Enter') {
														updateTokenValue(subToken.id, suggestion);
														selectedToken = null;
													}
												}}
											>
												{suggestion}
											</div>
										{/each}
									</Popover>
								</div>
							{:else if subToken.type == 'operator'}
								<div style:position="relative">
									<button
										class="bx--tile field-row-button"
										on:click={() => showOperatorSuggestions(subToken.id)}>{subToken.value}</button
									>
									<Popover open={selectedToken === subToken.id} align="bottom-left">
										{#each suggestions as suggestion}
											<div
												class="popover-item"
												role="button"
												tabindex="0"
												on:click={() => {
													updateTokenValue(subToken.id, suggestion);
													selectedToken = null;
												}}
												on:keydown={(e) => {
													if (e.key === 'Enter') {
														updateTokenValue(subToken.id, suggestion);
														selectedToken = null;
													}
												}}
											>
												{suggestion}
											</div>
										{/each}
									</Popover>
								</div>
							{:else if subToken.type == 'value'}
								{#if selectedToken === subToken.id}
									<div style:position="relative">
										<input
											class="bx--tile field-row-field"
											type="text"
											value={subToken.value}
											on:input={(e) => {
												selectedToken = subToken.id;
												setCustomSuggestions([e.currentTarget.value]);
											}}
											on:keydown={(e) => {
												if (e.key === 'Enter') {
													updateTokenValue(subToken.id, e.currentTarget.value);
													selectedToken = null;
												}
											}}
										/>
										<Popover open={selectedToken === subToken.id} align="bottom-left">
											{#each suggestions as suggestion}
												<div
													class="popover-item"
													role="button"
													tabindex="0"
													on:click={() => {
														updateTokenValue(subToken.id, suggestion);
														selectedToken = null;
													}}
													on:keydown={(e) => {
														if (e.key === 'Enter') {
															updateTokenValue(subToken.id, suggestion);
															selectedToken = null;
														}
													}}
												>
													{suggestion}
												</div>
											{/each}
										</Popover>
									</div>
								{:else}
									<button
										class="bx--tile field-row-button"
										on:click={() => {
											selectedToken = subToken.id;
										}}>{subToken.value}</button
									>
								{/if}
							{/if}
						{/each}
					</div>
				{/if}
			{/each}{/key}
		<button class="bx--tile field-row-button" on:click={addDefaultWhereTokens}>+</button>
		<span class="bx--tile field-row-field" />
	</div>
	<Button
		iconDescription="Execute query"
		icon={Play}
		on:click={() => onExecuteQuery(whereTokens.map((t) => t.tokens).flat())}
		size="field"
	/>
</div>

<style>
	.field-row {
		display: flex;
		flex-flow: wrap;
		align-content: flex-start;
		row-gap: 8px;
	}

	.field-row-label,
	.field-row-button,
	.field-row-field {
		display: flex;
		align-items: center;
		justify-content: space-between;
		flex: 1;
		flex-shrink: 0;
		padding: 0.75rem;
		background-color: white;
		border-radius: 2px;
		border: medium;
		margin-right: 4px;
		min-height: auto;
		min-width: auto;
	}

	.field-row-label {
		color: var(--cds-button-primary);
		flex: 0;
	}

	.field-row-button {
		color: var(--cds-text-primary);
		cursor: pointer;
		width: auto;
		flex: 0;
	}

	.field-row-form {
		align-items: flex-start;
		display: flex;
		flex-direction: row;
		position: relative;
		text-align: left;
	}

	.popover-item {
		padding: 6px 12px 6px 12px;
	}

	.popover-item:hover {
		background-color: var(--cds-tag-hover-blue);
	}
</style>
