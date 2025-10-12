export default function Card({
  className,
  ...props
}: React.ComponentProps<"div">) {
  return (
    <div
      data-slot="card"
      className={`bg-card text-card-foreground flex flex-col gap-2 rounded-xl border ${className}`}
      {...props}
    />
  );
}
