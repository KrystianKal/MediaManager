'use server'

import {Media} from "@/src/features/media/Media";
import {API_URL} from "@/src/lib/constants";

//todo handle api errors
export async function getAllMedia():Promise<Media[]> {
  let data = await fetch(`${API_URL}/media`);
  let response = await data.json();
  return response.value;
}
export async function getMediaById(id: string): Promise<Media> {
  let data = await fetch(`${API_URL}/media/${id}`);
  let response = await data.json();
  return response.value;
}
