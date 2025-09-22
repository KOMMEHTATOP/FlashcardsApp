(function () {
    window.dragDropInterop = {
        initializeDragDrop: function (authToken, containerSelector, itemSelector, apiEndpoint, trashSelector) {
            const container = document.querySelector(containerSelector);
            if (!container) return;

            const items = container.querySelectorAll(itemSelector);
            if (!items.length) return;

            let dragged = null;

            items.forEach(function (item, idx) {
                const dragHandle = item.querySelector('.drag-handle') || item;

                dragHandle.draggable = true;
                dragHandle.style.cursor = 'grab';

                dragHandle.addEventListener('dragstart', function (e) {
                    dragged = item;
                    e.dataTransfer.effectAllowed = 'move';
                    try {
                        e.dataTransfer.setData('text/plain', item.dataset.groupId || item.dataset.cardId);
                    } catch {}
                    item.style.opacity = '0.5';
                });

                item.addEventListener('dragend', function () {
                    this.style.opacity = '1';
                    dragged = null;
                });

                item.addEventListener('dragover', function (e) {
                    e.preventDefault();
                    e.stopPropagation();
                    if (containerSelector === '#groups-container') {
                        this.style.border = '2px solid #0d6efd';
                    } else {
                        this.style.borderTop = '2px solid #0d6efd';
                    }
                });

                item.addEventListener('dragleave', function () {
                    this.style.border = '';
                    this.style.borderTop = '';
                });

                item.addEventListener('drop', function (e) {
                    e.preventDefault();
                    e.stopPropagation();

                    this.style.border = '';
                    this.style.borderTop = '';

                    if (!dragged || dragged === this) return;

                    let newOrder = [];

                    if (containerSelector === '#groups-container') {
                        // Логика для горизонтальной сетки Bootstrap (Groups.razor)
                        const draggedCol = dragged.closest('.col-md-4, .col');
                        const targetCol = this.closest('.col-md-4, .col');

                        if (!draggedCol || !targetCol) return;

                        const draggedIndex = Array.from(container.children).indexOf(draggedCol);
                        const targetIndex = Array.from(container.children).indexOf(targetCol);

                        if (draggedIndex !== -1 && targetIndex !== -1 && draggedIndex !== targetIndex) {
                            if (draggedIndex < targetIndex) {
                                targetCol.parentNode.insertBefore(draggedCol, targetCol.nextSibling);
                            } else {
                                targetCol.parentNode.insertBefore(draggedCol, targetCol);
                            }

                            let order = 1;
                            container.querySelectorAll(itemSelector).forEach(function (el) {
                                const groupId = el.dataset.groupId;
                                if (groupId) {
                                    newOrder.push({ Id: groupId, Order: order++ });
                                }
                            });
                        }
                    } else {
                        // Логика для вертикального списка (NavMenu.razor)
                        const draggedIndex = Array.from(container.children).indexOf(dragged);
                        const targetIndex = Array.from(container.children).indexOf(this);

                        if (draggedIndex !== -1 && targetIndex !== -1 && draggedIndex !== targetIndex) {
                            if (draggedIndex < targetIndex) {
                                container.insertBefore(dragged, this.nextSibling);
                            } else {
                                container.insertBefore(dragged, this);
                            }

                            let order = 1;
                            container.querySelectorAll(itemSelector).forEach(function (el) {
                                const groupId = el.dataset.groupId;
                                if (groupId) {
                                    newOrder.push({ Id: groupId, Order: order++ });
                                }
                            });
                        }
                    }

                    if (newOrder.length === 0) return;

                    fetch(apiEndpoint, {
                        method: 'PUT',
                        headers: {
                            'Content-Type': 'application/json',
                            'Authorization': 'Bearer ' + authToken
                        },
                        body: JSON.stringify(newOrder)
                    }).then(r => {
                        if (r.ok) {
                            try {
                                DotNet.invokeMethodAsync('FlashcardsBlazorUI', 'NotifyGroupsReordered', containerSelector);
                            } catch (err) {
                                console.error('DotNet invoke failed:', err);
                            }
                        } else {
                            alert('Ошибка изменения порядка');
                            location.reload();
                        }
                    }).catch(err => {
                        console.error('Ошибка запроса:', err);
                        alert('Ошибка изменения порядка');
                        location.reload();
                    });
                });
            });

            // Обработка корзины
            const trash = document.querySelector(trashSelector);
            if (trash) {
                trash.addEventListener('dragover', e => {
                    e.preventDefault();
                    trash.style.backgroundColor = '#f8d7da';
                });

                trash.addEventListener('dragleave', () => {
                    trash.style.backgroundColor = '';
                });

                trash.addEventListener('drop', e => {
                    e.preventDefault();
                    trash.style.backgroundColor = '';

                    let id = null;
                    try {
                        id = e.dataTransfer.getData('text/plain');
                    } catch {}

                    if (!id || !dragged) return;

                    if (!confirm('Удалить группу?')) return;

                    fetch(`http://localhost:5153/api/group/${id}`, {
                        method: 'DELETE',
                        headers: {
                            'Authorization': 'Bearer ' + authToken
                        }
                    }).then(r => {
                        if (r.ok) {
                            const elementToRemove = containerSelector === '#groups-container'
                                ? dragged.closest('.col-md-4, .col')
                                : dragged;
                            elementToRemove?.remove();

                            try {
                                DotNet.invokeMethodAsync('FlashcardsBlazorUI', 'NotifyGroupsReordered', containerSelector);
                            } catch (err) {
                                console.error('DotNet invoke failed:', err);
                            }
                        } else {
                            alert('Ошибка удаления группы');
                        }
                    }).catch(err => {
                        console.error('Ошибка удаления:', err);
                        alert('Ошибка удаления группы');
                    });
                });
            }
        }
    };
})();