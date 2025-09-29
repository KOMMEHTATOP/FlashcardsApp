(function () {
    // Глобальная блокировка удаления
    let globalDeletionInProgress = false;

    window.dragDropInterop = {
        initializeDragDrop: function (authToken, containerSelector, itemSelector, apiEndpoint, trashSelector) {
            const container = document.querySelector(containerSelector);
            if (!container) {
                console.log("dragDrop: container not found:", containerSelector);
                return;
            }

            // Очищаем старые обработчики при реинициализации
            if (container.__dragDropInitialized) {
                console.log("dragDrop: cleaning old handlers for", containerSelector);
                this.cleanupDragDrop(containerSelector, itemSelector, trashSelector);
            }

            container.__dragDropInitialized = true;
            console.log("dragDrop: initializing for", containerSelector);

            // Нужен один таймер на контейнер
            if (!container.__notifyTimer) container.__notifyTimer = null;
            function notifyReordered(selector) {
                if (container.__notifyTimer) clearTimeout(container.__notifyTimer);
                container.__notifyTimer = setTimeout(() => {
                    console.log("dragDrop: notifyReordered ->", selector);
                    try {
                        DotNet.invokeMethodAsync('FlashcardsBlazorUI', 'NotifyGroupsReordered', selector);
                    } catch (err) {
                        console.error('DotNet invoke failed:', err);
                    }
                }, 250);
            }

            // Локальные переменные для drag&drop состояния
            let dragged = null;

            // Привязываем обработчики к элементам
            const items = container.querySelectorAll(itemSelector);
            if (!items.length) {
                console.log("dragDrop: no items for selector", itemSelector, "in", containerSelector);
                return;
            }

            items.forEach(function (item) {
                if (item.dataset.__ddInit === "1") return;
                item.dataset.__ddInit = "1";

                const dragHandle = item.querySelector('.drag-handle') || item;
                dragHandle.draggable = true;
                dragHandle.style.cursor = 'grab';

                // Переменные для отслеживания клика vs drag
                let mouseDownTime = 0;
                let mouseDownPos = { x: 0, y: 0 };
                let isDragStarted = false;
                let dragTimer = null;

                // Для карточек групп (не навигационное меню) добавляем обработку кликов
                if (containerSelector === '#groups-container') {
                    item.addEventListener('mousedown', function(e) {
                        mouseDownTime = Date.now();
                        mouseDownPos = { x: e.clientX, y: e.clientY };
                        isDragStarted = false;

                        // Таймер для определения начала drag (если мышь не отпустили за 150ms)
                        dragTimer = setTimeout(() => {
                            isDragStarted = true;
                        }, 150);
                    });

                    item.addEventListener('mouseup', function(e) {
                        if (e.target.closest('button')) {
                            return;
                        }

                        const timeDiff = Date.now() - mouseDownTime;
                        const distance = Math.sqrt(
                            Math.pow(e.clientX - mouseDownPos.x, 2) +
                            Math.pow(e.clientY - mouseDownPos.y, 2)
                        );

                        if (dragTimer) {
                            clearTimeout(dragTimer);
                            dragTimer = null;
                        }

                        // Если это быстрый клик (< 150ms) и мышь почти не двигалась (< 5px)
                        if (timeDiff < 150 && distance < 5 && !isDragStarted) {
                            const groupId = item.dataset.groupId;
                            if (groupId) {
                                window.location.href = `/groups/${groupId}/cards`;
                            }
                        }

                        isDragStarted = false;
                    });

                    item.addEventListener('mouseleave', function() {
                        if (dragTimer) {
                            clearTimeout(dragTimer);
                            dragTimer = null;
                        }
                    });
                }

                dragHandle.addEventListener('dragstart', function (e) {
                    if (globalDeletionInProgress) {
                        e.preventDefault();
                        return;
                    }

                    isDragStarted = true;
                    dragged = item;
                    e.dataTransfer.effectAllowed = 'move';
                    try {
                        e.dataTransfer.setData('text/plain', item.dataset.groupId || item.dataset.cardId);
                    } catch {}
                    item.style.opacity = '0.5';
                    item.__isDragging = true;
                });

                item.addEventListener('dragend', function () {
                    if (this.__isDragging) {
                        this.style.opacity = '1';
                        this.__isDragging = false;
                    }
                    if (dragged === this) {
                        dragged = null;
                    }
                    isDragStarted = false;
                });

                item.addEventListener('dragover', function (e) {
                    if (globalDeletionInProgress || !dragged || dragged === this) return;
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

                    if (globalDeletionInProgress || !dragged || dragged === this) return;

                    let newOrder = [];

                    if (containerSelector === '#groups-container') {
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
                                if (groupId) newOrder.push({ Id: groupId, Order: order++ });
                            });
                        }
                    } else {
                        const draggedIndex = Array.from(container.children).indexOf(dragged);
                        const targetIndex = Array.from(container.children).indexOf(this);
                        if (draggedIndex !== -1 && targetIndex !== -1 && draggedIndex !== targetIndex) {
                            if (draggedIndex < targetIndex) container.insertBefore(dragged, this.nextSibling);
                            else container.insertBefore(dragged, this);

                            let order = 1;
                            container.querySelectorAll(itemSelector).forEach(function (el) {
                                const groupId = el.dataset.groupId;
                                if (groupId) newOrder.push({ Id: groupId, Order: order++ });
                            });
                        }
                    }

                    if (newOrder.length === 0) return;

                    console.log("dragDrop: sending reorder:", newOrder.map(x => x.Id + ":" + x.Order).join(","));
                    fetch(apiEndpoint, {
                        method: 'PUT',
                        headers: { 'Content-Type': 'application/json', 'Authorization': 'Bearer ' + authToken },
                        body: JSON.stringify(newOrder)
                    }).then(r => {
                        if (r.ok) {
                            notifyReordered(containerSelector);
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

            // Инициализация корзины
            const trash = document.querySelector(trashSelector);
            if (trash && !trash.dataset.__ddInit) {
                trash.dataset.__ddInit = "1";

                trash.addEventListener('dragover', e => {
                    if (!globalDeletionInProgress && dragged) {
                        e.preventDefault();
                        trash.style.backgroundColor = '#f8d7da';
                    }
                });

                trash.addEventListener('dragleave', () => {
                    trash.style.backgroundColor = '';
                });

                trash.addEventListener('drop', e => {
                    e.preventDefault();
                    trash.style.backgroundColor = '';

                    // Глобальная проверка блокировки
                    if (globalDeletionInProgress || !dragged) {
                        console.log("dragDrop: удаление заблокировано или нет dragged элемента");
                        return;
                    }

                    let id = null;
                    try {
                        id = e.dataTransfer.getData('text/plain');
                    } catch {}

                    if (!id) {
                        console.log("dragDrop: не удалось получить ID элемента");
                        return;
                    }

                    if (!confirm('Удалить группу?')) return;

                    // Устанавливаем глобальную блокировку
                    globalDeletionInProgress = true;
                    console.log("dragDrop: начинаем удаление, блокировка установлена");

                    // Находим элемент для удаления
                    const elementToRemove = containerSelector === '#groups-container'
                        ? dragged.closest('.col-md-4, .col')
                        : dragged;

                    if (!elementToRemove) {
                        globalDeletionInProgress = false;
                        console.log("dragDrop: элемент для удаления не найден, блокировка снята");
                        return;
                    }

                    // Скрываем элемент сразу для лучшего UX
                    elementToRemove.style.opacity = '0.3';
                    elementToRemove.style.pointerEvents = 'none';

                    fetch(`http://localhost:5153/api/group/${id}`, {
                        method: 'DELETE',
                        headers: { 'Authorization': 'Bearer ' + authToken }
                    }).then(r => {
                        if (r.ok) {
                            // Успешное удаление - восстанавливаем элемент перед уведомлением Blazor
                            console.log("dragDrop: успешное удаление, восстанавливаем элемент и уведомляем Blazor");
                            elementToRemove.style.opacity = '1';
                            elementToRemove.style.pointerEvents = 'auto';
                            dragged = null;

                            // Уведомляем Blazor - он удалит элемент правильно
                            try {
                                DotNet.invokeMethodAsync('FlashcardsBlazorUI', 'NotifyGroupDeleted');
                            } catch (err) {
                                console.error('DotNet invoke failed:', err);
                            }
                        } else {
                            // Ошибка удаления - восстанавливаем элемент
                            console.log("dragDrop: ошибка удаления, восстанавливаем элемент");
                            elementToRemove.style.opacity = '1';
                            elementToRemove.style.pointerEvents = 'auto';
                            alert('Ошибка удаления группы');
                        }
                    }).catch(err => {
                        // Ошибка сети - восстанавливаем элемент
                        console.log("dragDrop: ошибка сети, восстанавливаем элемент");
                        elementToRemove.style.opacity = '1';
                        elementToRemove.style.pointerEvents = 'auto';
                        console.error('Ошибка удаления:', err);
                        alert('Ошибка удаления группы');
                    }).finally(() => {
                        // Снимаем глобальную блокировку
                        globalDeletionInProgress = false;
                        console.log("dragDrop: блокировка снята");
                    });
                });
            }

            console.log("dragDrop: init finished for", containerSelector);
        },

        cleanupDragDrop: function (containerSelector, itemSelector, trashSelector) {
            const container = document.querySelector(containerSelector);
            if (!container) return;

            // Очищаем флаги инициализации для элементов
            const items = container.querySelectorAll(itemSelector);
            items.forEach(item => {
                delete item.dataset.__ddInit;
                delete item.__isDragging;
            });

            // Очищаем флаг для корзины
            const trash = document.querySelector(trashSelector);
            if (trash) {
                delete trash.dataset.__ddInit;
            }

            // Очищаем таймер
            if (container.__notifyTimer) {
                clearTimeout(container.__notifyTimer);
                container.__notifyTimer = null;
            }

            console.log("dragDrop: cleanup completed for", containerSelector);
        }
    };
})();