import { useEditor, EditorContent } from '@tiptap/react';
import StarterKit from '@tiptap/starter-kit';
import Placeholder from '@tiptap/extension-placeholder';
import Underline from '@tiptap/extension-underline';
import Highlight from '@tiptap/extension-highlight';
import TextAlign from '@tiptap/extension-text-align';
import Subscript from '@tiptap/extension-subscript';
import Superscript from '@tiptap/extension-superscript';
import {
    Bold,
    Italic,
    Underline as UnderlineIcon,
    Strikethrough,
    Highlighter,
    List,
    ListOrdered,
    AlignLeft,
    AlignCenter,
    AlignRight,
    Subscript as SubscriptIcon,
    Superscript as SuperscriptIcon,
    Undo,
    Redo
} from 'lucide-react';
import { useEffect } from 'react';

interface RichTextEditorProps {
    content: string;
    onChange: (html: string) => void;
    placeholder?: string;
    maxLength?: number;
}

export default function RichTextEditor({
                                           content,
                                           onChange,
                                           placeholder = '',
                                           maxLength = 500,
                                       }: RichTextEditorProps) {
    const editor = useEditor({
        extensions: [
            StarterKit,
            Placeholder.configure({
                placeholder,
            }),
            Underline,
            Highlight,
            TextAlign.configure({
                types: ['heading', 'paragraph'],
            }),
            Subscript,
            Superscript,
        ],
        content,
        onUpdate: ({ editor }) => {
            onChange(editor.getHTML());
        },
    });

    useEffect(() => {
        if (editor && content !== editor.getHTML()) {
            editor.commands.setContent(content);
        }
    }, [content, editor]);

    if (!editor) return null;

    const textLength = editor.getText().length;

    return (
        <div className="border-2 border-purple-200 rounded-md focus-within:border-purple-500 focus-within:ring-2 focus-within:ring-purple-600 focus-within:ring-offset-2 bg-white/50">
            {/* Toolbar */}
            <div className="flex flex-wrap gap-1 p-2 border-b border-purple-200 bg-gray-50 rounded-t-md">
                <button
                    type="button"
                    onClick={() => editor.chain().focus().toggleBold().run()}
                    className={`p-2 rounded hover:bg-purple-100 text-gray-700 ${
                        editor.isActive('bold') ? 'bg-purple-200' : ''
                    }`}
                >
                    <Bold className="w-4 h-4" />
                </button>
                <button
                    type="button"
                    onClick={() => editor.chain().focus().toggleItalic().run()}
                    className={`p-2 rounded hover:bg-purple-100 text-gray-700 ${
                        editor.isActive('italic') ? 'bg-purple-200' : ''
                    }`}
                >
                    <Italic className="w-4 h-4" />
                </button>
                <button
                    type="button"
                    onClick={() => editor.chain().focus().toggleUnderline().run()}
                    className={`p-2 rounded hover:bg-purple-100 text-gray-700 ${
                        editor.isActive('underline') ? 'bg-purple-200' : ''
                    }`}
                >
                    <UnderlineIcon className="w-4 h-4" />
                </button>
                <button
                    type="button"
                    onClick={() => editor.chain().focus().toggleStrike().run()}
                    className={`p-2 rounded hover:bg-purple-100 text-gray-700 ${
                        editor.isActive('strike') ? 'bg-purple-200' : ''
                    }`}
                >
                    <Strikethrough className="w-4 h-4" />
                </button>
                <button
                    type="button"
                    onClick={() => editor.chain().focus().toggleHighlight().run()}
                    className={`p-2 rounded hover:bg-purple-100 text-gray-700 ${
                        editor.isActive('highlight') ? 'bg-purple-200' : ''
                    }`}
                >
                    <Highlighter className="w-4 h-4" />
                </button>

                <div className="w-px bg-purple-200 mx-1" />

                <button
                    type="button"
                    onClick={() => editor.chain().focus().toggleSubscript().run()}
                    className={`p-2 rounded hover:bg-purple-100 text-gray-700 ${
                        editor.isActive('subscript') ? 'bg-purple-200' : ''
                    }`}
                >
                    <SubscriptIcon className="w-4 h-4" />
                </button>
                <button
                    type="button"
                    onClick={() => editor.chain().focus().toggleSuperscript().run()}
                    className={`p-2 rounded hover:bg-purple-100 text-gray-700 ${
                        editor.isActive('superscript') ? 'bg-purple-200' : ''
                    }`}
                >
                    <SuperscriptIcon className="w-4 h-4" />
                </button>

                <div className="w-px bg-purple-200 mx-1" />

                <button
                    type="button"
                    onClick={() => editor.chain().focus().toggleBulletList().run()}
                    className={`p-2 rounded hover:bg-purple-100 text-gray-700 ${
                        editor.isActive('bulletList') ? 'bg-purple-200' : ''
                    }`}
                >
                    <List className="w-4 h-4" />
                </button>
                <button
                    type="button"
                    onClick={() => editor.chain().focus().toggleOrderedList().run()}
                    className={`p-2 rounded hover:bg-purple-100 text-gray-700 ${
                        editor.isActive('orderedList') ? 'bg-purple-200' : ''
                    }`}
                >
                    <ListOrdered className="w-4 h-4" />
                </button>

                <div className="w-px bg-purple-200 mx-1" />

                <button
                    type="button"
                    onClick={() => editor.chain().focus().setTextAlign('left').run()}
                    className={`p-2 rounded hover:bg-purple-100 text-gray-700 ${
                        editor.isActive({ textAlign: 'left' }) ? 'bg-purple-200' : ''
                    }`}
                >
                    <AlignLeft className="w-4 h-4" />
                </button>
                <button
                    type="button"
                    onClick={() => editor.chain().focus().setTextAlign('center').run()}
                    className={`p-2 rounded hover:bg-purple-100 text-gray-700 ${
                        editor.isActive({ textAlign: 'center' }) ? 'bg-purple-200' : ''
                    }`}
                >
                    <AlignCenter className="w-4 h-4" />
                </button>
                <button
                    type="button"
                    onClick={() => editor.chain().focus().setTextAlign('right').run()}
                    className={`p-2 rounded hover:bg-purple-100 text-gray-700 ${
                        editor.isActive({ textAlign: 'right' }) ? 'bg-purple-200' : ''
                    }`}
                >
                    <AlignRight className="w-4 h-4" />
                </button>

                <div className="flex-1" />

                <button
                    type="button"
                    onClick={() => editor.chain().focus().undo().run()}
                    disabled={!editor.can().undo()}
                    className="p-2 rounded hover:bg-purple-100 text-gray-700 disabled:opacity-50"
                >
                    <Undo className="w-4 h-4" />
                </button>
                <button
                    type="button"
                    onClick={() => editor.chain().focus().redo().run()}
                    disabled={!editor.can().redo()}
                    className="p-2 rounded hover:bg-purple-100 text-gray-700 disabled:opacity-50"
                >
                    <Redo className="w-4 h-4" />
                </button>
            </div>

            {/* Editor */}
            <EditorContent
                editor={editor}
                className="min-h-[100px] p-3 prose prose-sm max-w-none focus:outline-none [&_.tiptap]:outline-none text-gray-900"
            />

            {/* Character count */}
            <div className="px-3 py-1 text-xs text-gray-500 border-t border-purple-200">
                {textLength} / {maxLength} символов
            </div>
        </div>
    );
}