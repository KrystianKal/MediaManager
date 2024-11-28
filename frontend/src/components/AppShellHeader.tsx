import {AppShell, Box, Burger, Button, Group} from "@mantine/core";
import {ColorSchemeToggle} from "@/src/components/ColorSchemeToggle/ColorSchemeToggle";
import React from "react";
import SearchButton from "@/src/components/Search/SearchButton";
import {SearchSpotlight} from "@/src/components/Search/SearchSpotlight";
import {spotlight} from "@mantine/spotlight";

type AppHeaderProps = {mobileOpened: boolean, desktopOpened: boolean, toggleMobile: () => void, toggleDesktop: () => void};
export function AppHeader ({mobileOpened, desktopOpened, toggleMobile, toggleDesktop}: AppHeaderProps) {
  return (
  <AppShell.Header>
    <Group h="100%" px="md" justify={'space-between'} align="center">
      <Group>
        <Burger opened={mobileOpened} onClick={toggleMobile} hiddenFrom="sm" size="sm" />
        <Burger opened={desktopOpened} onClick={toggleDesktop} visibleFrom="sm" size="sm" />
      </Group>
      <SearchButton/>
      <ColorSchemeToggle />
    </Group>
  </AppShell.Header>
  )
}
