import { Helmet } from 'react-helmet-async';
import { useLocation } from 'react-router-dom';

interface SeoProps {
    title: string;
    description: string;
    type?: 'website' | 'article';
    name?: string;
    img?: string;
}

export const Seo = ({
                        title,
                        description,
                        type = 'website',
                        name = 'Flashcards Loop',
                        img
                    }: SeoProps) => {
    const location = useLocation();

    // Формируем каноническую ссылку (текущий домен + путь без query параметров)
    // Это говорит гуглу: "Вот эта страница — оригинал", даже если в URL есть ?ref=123
    const canonicalUrl = `https://flashcardsloop.org${location.pathname}`;

    return (
        <Helmet>
            {/* Standard metadata */}
            <title>{title}</title>
            <meta name='description' content={description} />
            <link rel="canonical" href={canonicalUrl} />

            {/* Open Graph / Facebook / Telegram */}
            <meta property="og:type" content={type} />
            <meta property="og:title" content={title} />
            <meta property="og:description" content={description} />
            <meta property="og:url" content={canonicalUrl} />
            <meta property="og:site_name" content={name} />
            {img && <meta property="og:image" content={img} />}

            {/* Twitter */}
            <meta name="twitter:card" content="summary_large_image" />
            <meta name="twitter:title" content={title} />
            <meta name="twitter:description" content={description} />
            {img && <meta name="twitter:image" content={img} />}
        </Helmet>
    );
};