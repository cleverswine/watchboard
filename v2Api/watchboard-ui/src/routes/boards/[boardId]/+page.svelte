<script lang="ts">
	import { Board, type Item } from '$lib';
	import DetailModal from './DetailModal.svelte';

	let { data } = $props();

	let board: Board = $state(data.board);
	let selectedItem: Item = $state(board.lists[0].items[0]);
	let modalOpen: boolean = $state(false);

	function selectItem(evt: MouseEvent) {
		const target = evt.target as HTMLButtonElement;
		const foundList = board.lists.find((item: { id: string }) => item.id === target.dataset.list);
		const foundItem = foundList?.items.find((item: { id: string }) => item.id === target.value);
		if (foundItem)
			selectedItem = foundItem;
		modalOpen = true;
	}
</script>

<h4>
	{board.name}
</h4>

{#each board.lists as list}
	<h6>
		{list.name} ({list.items.length})
	</h6>
	<div class="row">
		{#each list.items as witem}
			<div class="col-2 mb-3">
				<img class="img-thumbnail" src="/api/items/{witem.id}?a=/poster" alt="{witem.name} poster" />
				<button type="button" onclick={selectItem} value={witem.id} data-list="{witem.boardListId}"
								class="btn btn-primary" data-bs-toggle="modal" data-bs-target="#exampleModal">
					Launch demo modal
				</button>

			</div>
		{:else}
			<p class="mt-3 mb-4">Nothing here yet!</p>
		{/each}
	</div>
{/each}

<DetailModal open={modalOpen} onClosed={null} selectedItem={selectedItem}></DetailModal>
