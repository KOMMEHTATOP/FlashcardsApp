import { useEffect } from "react";
import { TITLE_APP } from "../test/data";

export default function useTitle(title: string) {
  useEffect(() => {
    document.title = TITLE_APP + " | " + title;
  }, [title]);
}
