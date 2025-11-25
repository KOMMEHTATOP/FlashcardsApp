import { TITLE_APP } from "@/shared/data";

export const LandingHelmet = () => {
    return (
        <>
            <title>{TITLE_APP} - Учись, развивайся, запоминай</title>
            <meta
                name="description"
                content={`${TITLE_APP} - современное веб-приложение для эффективного обучения с помощью карточек. Создавайте колоды, тренируйтесь и прокачивайтесь каждый день.`}
            />
            <meta
                name="keywords"
                content={`обучение, карточки, флешкарты, запоминание, обучение онлайн, ${TITLE_APP}`}
            />
            <meta name="robots" content="index, follow" />

            {/* опен граф */}
            <meta property="og:title" content={`${TITLE_APP} - учись и развивайся`} />
            <meta
                property="og:description"
                content="Эффективное обучение с помощью карточек и геймификации."
            />
            <meta property="og:type" content="website" />
            <meta property="og:url" content="https://flashcardsloop.org" />
            <meta property="og:image" content="/og-image.jpg" />

            {/* твитер */}
            <meta name="twitter:card" content="summary_large_image" />
            <meta
                name="twitter:title"
                content={`${TITLE_APP} — учись и развивайся`}
            />
            <meta
                name="twitter:description"
                content="Современное приложение для запоминания через карточки."
            />
            <meta name="twitter:image" content="/og-image.jpg" />
        </>
    );
};