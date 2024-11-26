import {AppShell,  Burger, Group} from "@mantine/core";
import {ColorSchemeToggle} from "@/src/components/ColorSchemeToggle/ColorSchemeToggle";
import React from "react";

type AppHeaderProps = {mobileOpened: boolean, desktopOpened: boolean, toggleMobile: () => void, toggleDesktop: () => void};
export function AppHeader ({mobileOpened, desktopOpened, toggleMobile, toggleDesktop}: AppHeaderProps) {
  return (
  <AppShell.Header>
    <Group h="100%" px="md">
      <Burger opened={mobileOpened} onClick={toggleMobile} hiddenFrom="sm" size="sm" />
      <Burger opened={desktopOpened} onClick={toggleDesktop} visibleFrom="sm" size="sm" />
      <ColorSchemeToggle />
    </Group>
  </AppShell.Header>
  )
}
