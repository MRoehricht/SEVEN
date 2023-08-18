<script lang="ts">
	import type { DqlProperty, DqlToken } from '$lib/types';
	import { Button, Checkbox, Popover, TextInput, Tile } from 'carbon-components-svelte';
	import { Play } from 'carbon-icons-svelte';
	import { onMount } from 'svelte';

	type ValidationError = {
		message: string;
		indexStart: number;
		indexEnd: number;
	};

	let operators = ['==', '!=', '>', '<', '>=', '<='];
	let logicals = ['AND', 'OR'];

	let tokens: DqlToken[] = [];
	let suggestions: string[] = [];
	let suggestionsOpen: boolean = false;
	let value: string = '';
	let invalidInput: boolean = false;
	let validationErrors: ValidationError[] = [];

	$: {
		tokenize(value);
		validateTokens(value);
		updateSuggestions(tokens);

		if (validationErrors.length == 1) {
			invalidInput = true;
		} else if (validationErrors.length > 0) {
			invalidInput = true;
		} else {
			invalidInput = false;
		}
	}

	function tokenize(value: string) {
		let words = value.split(/(\s+)/).filter((word) => word.trim().length > 0);

		tokens = [];
		let currentIndex = 0;

		words.forEach((token) => {
			const index = value.indexOf(token, currentIndex);

			if (dqlProperties.some((p) => p.propertyName === token)) {
				tokens.push({
					type: 'property',
					value: token,
					index
				});
			}

			if (operators.includes(token)) {
				tokens.push({
					type: 'operator',
					value: token,
					index
				});
			}

			if (logicals.includes(token)) {
				tokens.push({
					type: 'logical',
					value: token,
					index
				});
			}

			if (
				!dqlProperties.some((p) => p.propertyName === token) &&
				!operators.includes(token) &&
				!logicals.includes(token)
			) {
				tokens.push({
					type: 'value',
					value: token,
					index
				});
			}

			currentIndex = index + token.length;
		});
	}

	function validateTokens(input: string) {
		validationErrors = [];

		if (input.length === 0) {
			validationErrors.push({
				message: 'Expected a non empty string',
				indexStart: 0,
				indexEnd: 0
			});
		}

		for (let i = 0; i < tokens.length; i++) {
			const currentToken = tokens[i];
			const prevToken = tokens[i - 1];

			if (i === 0 && currentToken.type !== 'property') {
				validationErrors.push({
					message: `Expected a property at position ${currentToken.index}`,
					indexStart: currentToken.index,
					indexEnd: currentToken.index + currentToken.value.length
				});
			} else if (prevToken && prevToken.type === 'property' && currentToken.type !== 'operator') {
				validationErrors.push({
					message: `Expected an operator after property at position ${
						currentToken.index + currentToken.value.length
					}`,
					indexStart: currentToken.index,
					indexEnd: currentToken.index + currentToken.value.length
				});
			} else if (prevToken && prevToken.type === 'operator' && currentToken.type !== 'value') {
				validationErrors.push({
					message: `Expected a value after operator at position ${
						currentToken.index + currentToken.value.length
					}`,
					indexStart: currentToken.index,
					indexEnd: currentToken.index + currentToken.value.length
				});
			} else if (prevToken && prevToken.type === 'value' && currentToken.type !== 'logical') {
				validationErrors.push({
					message: `Expected a logical operator after value at position ${
						currentToken.index + currentToken.value.length
					}`,
					indexStart: currentToken.index,
					indexEnd: currentToken.index + currentToken.value.length
				});
			} else if (prevToken && prevToken.type === 'logical' && currentToken.type !== 'property') {
				validationErrors.push({
					message: `Expected a property after logical operator at position ${
						currentToken.index + currentToken.value.length
					}`,
					indexStart: currentToken.index,
					indexEnd: currentToken.index + currentToken.value.length
				});
			}
		}

		if (tokens.length > 0 && tokens[tokens.length - 1].type === 'property') {
			validationErrors.push({
				message: `Expected a operator after property at position ${
					tokens[tokens.length - 1].index + tokens[tokens.length - 1].value.length
				}`,
				indexStart: tokens[tokens.length - 1].index,
				indexEnd: tokens[tokens.length - 1].index + tokens[tokens.length - 1].value.length
			});
		}

		if (tokens.length > 0 && tokens[tokens.length - 1].type === 'operator') {
			validationErrors.push({
				message: `Expected a value after operator at position ${
					tokens[tokens.length - 1].index + tokens[tokens.length - 1].value.length
				}`,
				indexStart: tokens[tokens.length - 1].index,
				indexEnd: tokens[tokens.length - 1].index + tokens[tokens.length - 1].value.length
			});
		}

		if (tokens.length > 0 && tokens[tokens.length - 1].type === 'logical') {
			validationErrors.push({
				message: `Expected a property after logical operator at position ${
					tokens[tokens.length - 1].index + tokens[tokens.length - 1].value.length
				}`,
				indexStart: tokens[tokens.length - 1].index,
				indexEnd: tokens[tokens.length - 1].index + tokens[tokens.length - 1].value.length
			});
		}
	}

	function updateSuggestions(tokens: DqlToken[]) {
		let lastToken = tokens[tokens.length - 1];

		suggestions = [];

		if (!lastToken) {
			suggestions = dqlProperties.map((p) => p.propertyName).flat();
			return;
		}

		let prevToken = tokens[tokens.length - 2];

		if (lastToken.type === 'property') {
			suggestions = operators;
		} else if (lastToken.type === 'operator') {
			if (prevToken.type === 'property') {
				suggestions = dqlProperties
					.filter((p) => p.propertyName === prevToken.value)
					.map((p) => p.propertyValues)
					.flat();
			}
		} else if (lastToken.type === 'value') {
			if (prevToken && prevToken.type === 'operator') {
				suggestions = logicals;
			}

			let matchingProperties = dqlProperties.filter((p) =>
				p.propertyName.includes(lastToken.value)
			);

			if (matchingProperties.length > 0) {
				suggestions = matchingProperties.map((p) => p.propertyName).flat();
			}
		} else if (lastToken.type === 'logical') {
			suggestions = dqlProperties.map((p) => p.propertyName).flat();
		}

		suggestionsOpen = suggestions.length > 0;
	}

	export let dqlProperties: DqlProperty[];
	export let onExecuteQuery: (tokens: DqlToken[]) => void;
</script>

<div>
	<div class="query-input">
		<div class="query-tooltip">
			<TextInput
				placeholder="Enter a DQL query"
				bind:value
				bind:invalid={invalidInput}
				on:focus={() => {
					updateSuggestions(tokens);
				}}
				on:blur={() => {
					suggestions = [];
				}}
			/>
			<Popover open align="bottom-left">
				{#each validationErrors as error}
					<div class="popover-item error">{error.message}</div>
				{/each}
				{#each suggestions as suggestion}
					<div class="popover-item">{suggestion}</div>
				{/each}
			</Popover>
		</div>
		<Button
			iconDescription="Execute query"
			icon={Play}
			on:click={() => onExecuteQuery(tokens)}
			size="field"
			disabled={invalidInput}
		/>
	</div>
</div>

<style>
	.query-input {
		display: flex;
		flex-direction: row;
		margin-top: 6px;
		gap: 6px;
	}

	.query-tooltip {
		position: relative;
		display: flex;
		flex-direction: column;
		flex: 1;
	}

	.popover-item {
		padding: 12px;
		width: 100%;
	}

	.error {
		color: white;
		background-color: lightcoral;
	}
</style>
