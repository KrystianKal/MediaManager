import {Directory} from "@/src/features/directories/Directory";
import {API_URL} from "@/src/lib/constants";

export async function getAllDirectories(): Promise<Directory[]> {
  let data = await fetch(`${API_URL}/directories`);
  let response = await data.json();
  return response.value;
}
