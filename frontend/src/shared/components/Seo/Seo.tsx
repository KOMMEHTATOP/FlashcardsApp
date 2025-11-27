import { Helmet } from 'react-helmet-async';
import { useLocation } from 'react-router-dom';

interface SeoProps {
    title: string;
    description: string;
    type?: 'website' | 'article';
    name?: string;
    img?: string;
    jsonLd?: string; 
}

export const Seo = ({
                        title,
                        description,
                        type = 'website',
                        name = 'Flashcards Loop',
                        img,
                        jsonLd 
                    }: SeoProps) => {
    const location = useLocation();
    const canonicalUrl = `https://flashcardsloop.org${location.pathname}`;

    return (
        <Helmet>
            <title>{title}</title>
            <meta name='description' content={description} />
            <link rel="canonical" href={canonicalUrl} />

            <meta property="og:type" content={type} />
            <meta property="og:title" content={title} />
            <meta property="og:description" content={description} />
            <meta property="og:url" content={canonicalUrl} />
            <meta property="og:site_name" content={name} />
            {img && <meta property="og:image" content={img} />}

            <meta name="twitter:card" content="summary_large_image" />
            <meta name="twitter:title" content={title} />
            <meta name="twitter:description" content={description} />
            {img && <meta name="twitter:image" content={img} />}

            {jsonLd && (
                <script type="application/ld+json">
                    {jsonLd}
                </script>
            )}
        </Helmet>
    );
};