import { getAllMedia } from "@/src/features/media/actions";
import { MediaGridClient } from "./MediaGridClient";

export const GRID_TYPES = {1: "Masonry" , 2: "Even"} as const
export async function MediaGrid() {
  const media = await getAllMedia();
  return <MediaGridClient initialMedia={media} />;
}
