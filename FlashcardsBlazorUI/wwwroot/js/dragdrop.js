window.dragDropInterop = {
    initializeDragDrop: function() {
        console.log("=== Начало инициализации drag & drop ===");

        const groupCards = document.querySelectorAll('.group-card');
        console.log(`Найдено карточек групп: ${groupCards.length}`);

        const trashZone = document.querySelector('.trash-zone');
        console.log(`Корзина найдена: ${trashZone ? 'да' : 'нет'}`);

        // Делаем карточки групп перетаскиваемыми
        groupCards.forEach((card, index) => {
            console.log(`Настройка карточки ${index + 1}:`, card.dataset.groupId);
            card.draggable = true;

            card.addEventListener('dragstart', function(e) {
                const groupId = this.dataset.groupId;
                e.dataTransfer.setData('text/plain', groupId);
                this.style.opacity = '0.5';
                console.log('DRAG START - ID группы:', groupId);
            });

            card.addEventListener('dragend', function(e) {
                this.style.opacity = '1';
                console.log('DRAG END');
            });
        });

        // Настраиваем корзину
        if (trashZone) {
            trashZone.addEventListener('dragover', function(e) {
                e.preventDefault();
                this.style.backgroundColor = '#f8d7da';
                console.log('DRAG OVER trash zone');
            });

            trashZone.addEventListener('dragleave', function(e) {
                this.style.backgroundColor = '';
                console.log('DRAG LEAVE trash zone');
            });

            trashZone.addEventListener('drop', function(e) {
                e.preventDefault();
                this.style.backgroundColor = '';
                const groupId = e.dataTransfer.getData('text/plain');
                console.log('DROP на корзину! Group ID:', groupId);
                DotNet.invokeMethodAsync('FlashcardsBlazorUI', 'DeleteGroupFromJS', groupId);
            });
        }

        console.log("=== Инициализация завершена ===");
    }
};