import { useCallback } from 'react';

import { useId } from '@mantine/hooks';
import { modals } from '@mantine/modals';
import { ModalSettings } from '@mantine/modals/lib/context';

export function useContextModal(props: ModalSettings) {
  const { modalId, onClose, ...other } = props;

  const modalKey = modalId ?? useId();

  const openModal = useCallback(() => {
    modals.open({
      ...other,
      modalId: modalKey,
    });
  }, [other, modalKey]);

  const closeModal = useCallback(() => {
    modals.close(modalKey);
    onClose && onClose();
  }, [onClose, modalKey]);

  return {
    open: openModal,
    close: closeModal,
  };
}
