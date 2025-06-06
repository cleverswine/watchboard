<script lang="ts">
	import { Board, type Item } from '$lib';

	let { data } = $props();

	let board: Board = $state(data.board);
	let selectedItem: Item = $state(board.lists[0].items[0]);

	function selectItem(evt: MouseEvent) {
		const target = evt.target as HTMLButtonElement;
		const foundList = board.lists.find((item: { id: string }) => item.id === target.dataset.list);
		const foundItem = foundList?.items.find((item: { id: string }) => item.id === target.value);
		if (foundItem)
			selectedItem = foundItem;
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

<!-- Modal -->
<div class="modal fade" id="exampleModal" tabindex="-1" aria-labelledby="exampleModalLabel" aria-hidden="true">
	<div class="modal-dialog">
		<div class="modal-content">
			<div class="modal-header">
				<h1 class="modal-title fs-5" id="exampleModalLabel">{selectedItem?.name}</h1>
				<button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
			</div>
			<div class="modal-body">
				...
			</div>
			<div class="modal-footer">
				<button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Close</button>
				<button type="button" class="btn btn-primary">Save changes</button>
			</div>
		</div>
	</div>
</div>