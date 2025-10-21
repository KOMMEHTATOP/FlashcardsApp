interface SwitchProps extends React.HTMLProps<HTMLInputElement> {
  checked: boolean;
  onCheckedChange: (checked: boolean) => void;
}

export default function Switch({
  checked,
  onCheckedChange,
  ...props
}: SwitchProps) {
  return (
    <input
      type="checkbox"
      className="toggle bg-gray-400 checked:bg-neutral"
      checked={checked}
      onChange={(e) => onCheckedChange(e.target.checked)}
      {...props}
    />
  );
}
