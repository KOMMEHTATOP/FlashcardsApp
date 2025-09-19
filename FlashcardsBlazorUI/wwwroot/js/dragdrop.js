(function () {
    window.dragDropInterop = {
        initializeDragDrop: function (authToken, containerSelector, itemSelector, apiEndpoint, trashSelector) {
            console.log(`=== dragdrop: init for ${containerSelector} ===`);

            const container = document.querySelector(containerSelector);
            if (!container) {
                console.log(`dragdrop: ${containerSelector} не найден`);
                return;
            }

            const items = container.querySelectorAll(itemSelector);
            console.log(`dragdrop: найдено элементов = ${items.length}`);
            if (!items.length) return;

            let dragged = null;

            items.forEach(function (item, idx) {
                // Унифицированная логика для drag handle
                const dragHandle = item.querySelector('.drag-handle') || item;

                dragHandle.draggable = true;
                dragHandle.style.cursor = 'grab';
                console.log(`Adding dragstart to: ${item.className} [${item.dataset.groupId || item.dataset.cardId}]`);

                dragHandle.addEventListener('dragstart', function (e) {
                    console.log('Dragstart event fired for:', item.className);
                    dragged = item;
                    e.dataTransfer.effectAllowed = 'move';
                    try {
                        e.dataTransfer.setData('text/plain', item.dataset.groupId || item.dataset.cardId);
                    } catch {}
                    item.style.opacity = '0.5';
                });

                item.addEventListener('dragend', function () {
                    console.log('Dragend event fired');
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
                    console.log('Drop event fired');
                    console.log('Container selector:', containerSelector);

                    this.style.border = '';
                    this.style.borderTop = '';

                    if (!dragged) {
                        console.log('No dragged element');
                        return;
                    }

                    if (dragged === this) {
                        console.log('Same element - skipping');
                        return;
                    }

                    console.log('Starting reorder logic');

                    if (containerSelector === '#groups-container') {
                        console.log('Using Groups logic');
                        // Логика для горизонтальной сетки Bootstrap (Groups.razor)
                        const draggedCol = dragged.closest('.col-md-4, .col');
                        const targetCol = this.closest('.col-md-4, .col');

                        if (!draggedCol || !targetCol) {
                            console.log('Could not find col elements');
                            return;
                        }

                        const draggedIndex = Array.from(container.children).indexOf(draggedCol);
                        const targetIndex = Array.from(container.children).indexOf(targetCol);

                        console.log(`Groups drop: dragged=${draggedIndex}, target=${targetIndex}`);

                        if (draggedIndex !== -1 && targetIndex !== -1 && draggedIndex !== targetIndex) {
                            if (draggedIndex < targetIndex) {
                                targetCol.parentNode.insertBefore(draggedCol, targetCol.nextSibling);
                            } else {
                                targetCol.parentNode.insertBefore(draggedCol, targetCol);
                            }
                            console.log('Groups: DOM reordered');
                        }
                    } else {
                        console.log('Using NavMenu logic');
                        // Логика для вертикального списка (NavMenu.razor)
                        const draggedIndex = Array.from(container.children).indexOf(dragged);
                        const targetIndex = Array.from(container.children).indexOf(this);

                        console.log(`NavMenu drop: dragged=${draggedIndex}, target=${targetIndex}`);

                        if (draggedIndex !== -1 && targetIndex !== -1 && draggedIndex !== targetIndex) {
                            console.log('Attempting DOM reorder...');
                            if (draggedIndex < targetIndex) {
                                container.insertBefore(dragged, this.nextSibling);
                            } else {
                                container.insertBefore(dragged, this);
                            }
                            console.log('NavMenu: DOM reordered');
                        } else {
                            console.log('NavMenu reorder conditions not met:', {
                                draggedIndex,
                                targetIndex,
                                sameElement: dragged === this
                            });
                        }
                    }

                    // Сохранение нового порядка
                    const reorderData = [];
                    let order = 1;
                    container.querySelectorAll(itemSelector).forEach(function (el) {
                        const groupId = el.dataset.groupId;
                        if (groupId) {
                            reorderData.push({ Id: groupId, Order: order++ });
                        }
                    });

                    console.log('Reorder data:', reorderData);

                    if (reorderData.length === 0) {
                        console.log('No reorder data, skipping API call');
                        return;
                    }

                    fetch(apiEndpoint, {
                        method: 'PUT',
                        headers: {
                            'Content-Type': 'application/json',
                            'Authorization': 'Bearer ' + authToken
                        },
                        body: JSON.stringify(reorderData)
                    }).then(r => {
                        if (r.ok) {
                            console.log('Порядок обновлен');
                            try {
                                DotNet.invokeMethodAsync('FlashcardsBlazorUI', 'NotifyGroupsReordered');
                            } catch (err) {
                                console.log('DotNet invoke failed:', err);
                            }
                        } else {
                            alert('Ошибка изменения порядка');
                        }
                    }).catch(err => {
                        console.error('Ошибка запроса:', err);
                        alert('Ошибка изменения порядка');
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
                            console.log('Группа удалена');
                            // Удаляем правильный элемент в зависимости от контейнера
                            const elementToRemove = containerSelector === '#groups-container'
                                ? dragged.closest('.col-md-4, .col')
                                : dragged;
                            elementToRemove?.remove();

                            try {
                                DotNet.invokeMethodAsync('FlashcardsBlazorUI', 'NotifyGroupDeleted');
                            } catch (err) {
                                console.log('DotNet invoke failed:', err);
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

            console.log(`=== dragdrop: ready for ${containerSelector} ===`);
        }
    };
})();